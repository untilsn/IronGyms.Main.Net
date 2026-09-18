using IronGyms.Api.DTOs;
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
        var result = await _authService.RegisterMemberAsync(dto);
        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
        return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
        return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(rawRefreshToken))
            return Unauthorized(new { message = "Không tìm thấy refresh token" });

        var result = await _authService.RefreshAsync(rawRefreshToken);
        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
        return Ok(new { accessToken = result.AccessToken, accessTokenExpiresAt = result.AccessTokenExpiresAt });
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
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = "/api/auth"
        });
    }
}