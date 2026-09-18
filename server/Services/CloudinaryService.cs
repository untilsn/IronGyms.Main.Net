using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using IronGyms.Api.Configuration;
using IronGyms.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace IronGyms.Api.Services;

// Service upload/xoá ảnh dùng CHUNG cho mọi tính năng cần ảnh (avatar, product image...).
// "folder" để phân loại ảnh trên Cloudinary theo mục đích, vd "avatars", "products".
public interface ICloudinaryService
{
    Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file, string folder);
    Task DeleteImageAsync(string publicId);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public CloudinaryService(IOptions<CloudinaryOptions> options)
    {
        var o = options.Value;
        var account = new Account(o.CloudName, o.ApiKey, o.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file, string folder)
    {
        ValidateFile(file);

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"irongyms/{folder}",
            // Resize + crop vuông tập trung vào mặt - hợp cho avatar; với product folder sau này
            // có thể truyền transformation khác qua tham số nếu cần, chưa cần thiết ở bước này.
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw ApiException.BadGateway($"Upload ảnh thất bại: {result.Error.Message}");

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        await _cloudinary.DestroyAsync(deleteParams);
        // Không throw nếu xoá thất bại - ảnh cũ "mồ côi" trên Cloudinary không ảnh hưởng logic
        // chính, chỉ tốn dung lượng lưu trữ, chấp nhận được để không chặn luồng update chính.
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file.Length == 0)
            throw ApiException.BadRequest("File ảnh trống");

        if (file.Length > MaxFileSizeBytes)
            throw ApiException.BadRequest("File ảnh vượt quá 5MB");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw ApiException.BadRequest("Chỉ chấp nhận file JPEG, PNG hoặc WEBP");
    }
}