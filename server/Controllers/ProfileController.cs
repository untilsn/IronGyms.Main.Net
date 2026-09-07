using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Extensions;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize] // bất kỳ role nào đã đăng nhập đều gọi được, chỉ thao tác trên chính mình
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var result = await _profileService.GetMyProfileAsync(User.GetUserId());
            return Ok(result);
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto dto)
    {
        try
        {
            var result = await _profileService.UpdateProfileAsync(User.GetUserId(), dto);
            return Ok(result);
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
    {
        try
        {
            await _profileService.ChangePasswordAsync(User.GetUserId(), dto);
            return Ok(new { message = "Đổi mật khẩu thành công, vui lòng đăng nhập lại trên các thiết bị khác" });
        }
        catch (AuthException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}