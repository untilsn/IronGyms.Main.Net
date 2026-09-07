using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;
    private const string RefreshTokenCookieName = "refreshToken";

    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            var result = await _authService.RegisterMemberAsync(dto);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // Không nhận refreshToken từ body nữa - đọc trực tiếp từ cookie mà trình duyệt/Postman tự gửi lên.
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(rawRefreshToken))
            return Unauthorized(new { message = "Không tìm thấy refresh token" });

        try
        {
            var result = await _authService.RefreshAsync(rawRefreshToken);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(rawRefreshToken))
            await _authService.LogoutAsync(rawRefreshToken);

        Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions { Path = "/api/auth" });
        return Ok(new { message = "Đã đăng xuất" });
    }

    private void SetRefreshTokenCookie(string rawRefreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append(RefreshTokenCookieName, rawRefreshToken, new CookieOptions
        {
            HttpOnly = true,                 // JS không đọc được, kể cả qua XSS
            Secure = !_env.IsDevelopment(),  // localhost http vẫn gửi được nếu để false lúc dev
            SameSite = SameSiteMode.Lax,     // localhost:3000 và localhost:5000 vẫn tính là "same site"
            Expires = expiresAt,
            Path = "/api/auth"               // chỉ gửi cookie này khi gọi đúng nhóm route /api/auth/*
        });
    }
}