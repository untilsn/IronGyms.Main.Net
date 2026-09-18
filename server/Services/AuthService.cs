using IronGyms.Api.Configuration;
using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IronGyms.Api.Services;

public interface IAuthService
{
    // Chỉ dùng cho Member tự đăng ký ở trang client.
    // Staff/Trainer/Admin do Admin tạo tài khoản qua API riêng (chưa làm ở scope này).
    Task<AuthResponseDto> RegisterMemberAsync(RegisterRequestDto dto);

    // Dùng chung cho cả 4 role - frontend tự quyết định route dựa vào Role trả về trong JWT.
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);

    Task<AuthResponseDto> RefreshAsync(string rawRefreshToken);

    Task LogoutAsync(string rawRefreshToken);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AppDbContext db, ITokenService tokenService, IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterMemberAsync(RegisterRequestDto dto)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            throw ApiException.BadRequest("Email đã được sử dụng");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Member,
            IsActive = true
        };

        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = dto.FullName,
        };

        var memberDetail = new MemberDetail
        {
            ProfileId = profile.Id
        };

        _db.Users.Add(user);
        _db.Profiles.Add(profile);
        _db.MemberDetails.Add(memberDetail);
        await _db.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // Không tách 2 lỗi "sai email" vs "sai password" để tránh lộ thông tin email nào tồn tại.
        if (user == null || user.PasswordHash == null ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw ApiException.Unauthorized("Email hoặc mật khẩu không đúng");
        }

        if (!user.IsActive)
            throw ApiException.Forbidden("Tài khoản đã bị khoá");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponseDto> RefreshAsync(string rawRefreshToken)
    {
        var tokenHash = _tokenService.HashToken(rawRefreshToken);
        var existingToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (existingToken == null)
            throw ApiException.Unauthorized("Refresh token không hợp lệ");

        if (existingToken.IsRevoked)
        {
            // Token đã bị revoke mà vẫn có người dùng lại -> dấu hiệu bị đánh cắp.
            // Revoke toàn bộ refresh token còn sống của user này cho an toàn.
            var activeTokens = await _db.RefreshTokens
                .Where(rt => rt.UserId == existingToken.UserId && rt.RevokedAt == null)
                .ToListAsync();
            foreach (var t in activeTokens) t.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            throw ApiException.Unauthorized("Refresh token đã bị thu hồi, vui lòng đăng nhập lại");
        }

        if (existingToken.IsExpired)
            throw ApiException.Unauthorized("Refresh token đã hết hạn, vui lòng đăng nhập lại");

        if (!existingToken.User.IsActive)
            throw ApiException.Forbidden("Tài khoản đã bị khoá");

        // Rotation: revoke token cũ, phát hành token mới
        var newRawRefreshToken = _tokenService.GenerateRefreshToken();
        var newTokenHash = _tokenService.HashToken(newRawRefreshToken);
        var newExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByTokenHash = newTokenHash;

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existingToken.UserId,
            TokenHash = newTokenHash,
            ExpiresAt = newExpiresAt
        };
        _db.RefreshTokens.Add(newRefreshToken);

        var accessToken = _tokenService.GenerateAccessToken(existingToken.User);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            RefreshToken = newRawRefreshToken,
            RefreshTokenExpiresAt = newExpiresAt
        };
    }

    public async Task LogoutAsync(string rawRefreshToken)
    {
        var tokenHash = _tokenService.HashToken(rawRefreshToken);
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (token != null && token.RevokedAt == null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        // Không throw lỗi nếu token không tồn tại/đã revoke - logout luôn coi như thành công.
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var rawRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashToken(rawRefreshToken);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = refreshExpiresAt
        });
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }
}