using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IMemberService
{
    Task<List<MemberListItemDto>> GetAllAsync(string? search, bool? isActive);
    Task<MemberDetailDto> GetByIdAsync(Guid userId);
    Task SetActiveStatusAsync(Guid userId, bool isActive);
}

public class MemberService : IMemberService
{
    private readonly AppDbContext _db;

    public MemberService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MemberListItemDto>> GetAllAsync(string? search, bool? isActive)
    {
        var query = _db.Profiles
            .Include(p => p.User)
            .Where(p => p.User.Role == UserRole.Member);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.FullName.Contains(search) || p.User.Email.Contains(search));

        if (isActive.HasValue)
            query = query.Where(p => p.User.IsActive == isActive.Value);

        var profiles = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return profiles.Select(MapToListItem).ToList();
    }

    public async Task<MemberDetailDto> GetByIdAsync(Guid userId)
    {
        var profile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.User.Role == UserRole.Member);

        if (profile == null)
            throw ApiException.NotFound("Không tìm thấy member");

        return MapToDetail(profile);
    }

    public async Task SetActiveStatusAsync(Guid userId, bool isActive)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRole.Member);
        if (user == null)
            throw ApiException.NotFound("Không tìm thấy member");

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!isActive)
        {
            // Khoá tài khoản thì thu hồi luôn mọi refresh token đang sống - đá ra khỏi mọi thiết bị ngay,
            // không đợi access token cũ hết hạn mới thật sự mất quyền truy cập.
            var activeTokens = await _db.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();
            foreach (var t in activeTokens) t.RevokedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    private static MemberListItemDto MapToListItem(Profile profile) => new()
    {
        UserId = profile.UserId,
        Email = profile.User.Email,
        FullName = profile.FullName,
        PhoneNumber = profile.PhoneNumber,
        Gender = profile.Gender,
        IsActive = profile.User.IsActive,
        CreatedAt = profile.CreatedAt
    };

    private static MemberDetailDto MapToDetail(Profile profile) => new()
    {
        UserId = profile.UserId,
        Email = profile.User.Email,
        FullName = profile.FullName,
        PhoneNumber = profile.PhoneNumber,
        Gender = profile.Gender,
        IsActive = profile.User.IsActive,
        CreatedAt = profile.CreatedAt,
        DateOfBirth = profile.DateOfBirth,
        AddressLine = profile.AddressLine,
        Ward = profile.Ward,
        District = profile.District,
        Province = profile.Province,
        AvatarUrl = profile.AvatarUrl,
        IsEmailVerified = profile.User.IsEmailVerified,
        UpdatedAt = profile.UpdatedAt
    };
}