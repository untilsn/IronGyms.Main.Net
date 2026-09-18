using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using IronGyms.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IProfileService
{
    Task<ProfileResponseDto> GetMyProfileAsync(Guid userId);
    Task<ProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto dto);
    Task<ProfileResponseDto> UpdateAvatarAsync(Guid userId, IFormFile file);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto);
}

public class ProfileService : IProfileService
{
    private readonly AppDbContext _db;
    private readonly ICloudinaryService _cloudinaryService;

    public ProfileService(AppDbContext db, ICloudinaryService cloudinaryService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ProfileResponseDto> GetMyProfileAsync(Guid userId)
    {
        var profile = await FindProfileOrThrowAsync(userId);
        return MapToDto(profile);
    }

    public async Task<ProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto dto)
    {
        var profile = await FindProfileOrThrowAsync(userId);

        profile.FullName = dto.FullName;
        profile.PhoneNumber = dto.PhoneNumber;
        profile.Gender = dto.Gender;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(profile);
    }

    public async Task<ProfileResponseDto> UpdateAvatarAsync(Guid userId, IFormFile file)
    {
        var profile = await FindProfileOrThrowAsync(userId);
        var oldPublicId = profile.AvatarPublicId;

        var (url, publicId) = await _cloudinaryService.UploadImageAsync(file, "avatars");

        profile.AvatarUrl = url;
        profile.AvatarPublicId = publicId;
        profile.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Chỉ xoá ảnh cũ SAU KHI đã lưu ảnh mới thành công vào DB -
        // tránh trường hợp SaveChanges lỗi mà ảnh cũ đã bị xoá mất trên Cloudinary.
        if (!string.IsNullOrEmpty(oldPublicId))
            await _cloudinaryService.DeleteImageAsync(oldPublicId);

        return MapToDto(profile);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw ApiException.NotFound("Không tìm thấy tài khoản");

        if (user.PasswordHash == null)
            throw ApiException.BadRequest("Tài khoản này đăng nhập bằng Google, chưa có mật khẩu để đổi");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw ApiException.Unauthorized("Mật khẩu hiện tại không đúng");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();
        foreach (var t in activeTokens) t.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private async Task<Profile> FindProfileOrThrowAsync(Guid userId)
    {
        var profile = await _db.Profiles
            .Include(p => p.User)
            .Include(p => p.TrainerDetail)
            .Include(p => p.StaffDetail)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw ApiException.NotFound("Không tìm thấy hồ sơ người dùng");

        return profile;
    }

    private static ProfileResponseDto MapToDto(Profile profile)
    {
        return new ProfileResponseDto
        {
            UserId = profile.UserId,
            Email = profile.User.Email,
            Role = profile.User.Role,
            FullName = profile.FullName,
            PhoneNumber = profile.PhoneNumber,
            Gender = profile.Gender,
            DateOfBirth = profile.DateOfBirth,
            AvatarUrl = profile.AvatarUrl,
            Specialization = profile.TrainerDetail?.Specialization,
            Bio = profile.TrainerDetail?.Bio,
            HireDate = profile.StaffDetail?.HireDate
        };
    }
}