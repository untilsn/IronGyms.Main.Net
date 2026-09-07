using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IProfileService
{
    Task<ProfileResponseDto> GetMyProfileAsync(Guid userId);
    Task<ProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto);
}

public class ProfileService : IProfileService
{
    private readonly AppDbContext _db;

    public ProfileService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProfileResponseDto> GetMyProfileAsync(Guid userId)
    {
        var profile = await FindProfileOrThrowAsync(userId);
        return MapToDto(profile);
    }

    public async Task<ProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto dto)
    {
        var profile = await FindProfileOrThrowAsync(userId);

        // Chỉ update field chung - field đặc thù role (Specialization, HireDate...)
        // để dành cho API riêng sau này, không đụng ở đây.
        profile.FullName = dto.FullName;
        profile.PhoneNumber = dto.PhoneNumber;
        profile.Gender = dto.Gender;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.AddressLine = dto.AddressLine;
        profile.Ward = dto.Ward;
        profile.District = dto.District;
        profile.Province = dto.Province;
        profile.UpdatedAt = DateTime.UtcNow;


        await _db.SaveChangesAsync();
        return MapToDto(profile);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new AuthException("Mật khẩu xác nhận không khớp", 400);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new AuthException("Không tìm thấy tài khoản", 404);

        if (user.PasswordHash == null)
            throw new AuthException(
                "Tài khoản này đăng nhập bằng Google, chưa có mật khẩu để đổi",
                400
            );

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new AuthException("Mật khẩu hiện tại không đúng", 401);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var t in activeTokens)
            t.RevokedAt = DateTime.UtcNow;

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
            throw new AuthException("Không tìm thấy hồ sơ người dùng", 404);

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
            AddressLine = profile.AddressLine,
            Ward = profile.Ward,
            District = profile.District,
            Province = profile.Province,
            Specialization = profile.TrainerDetail?.Specialization,
            Bio = profile.TrainerDetail?.Bio,
            HireDate = profile.StaffDetail?.HireDate
        };
    }
}