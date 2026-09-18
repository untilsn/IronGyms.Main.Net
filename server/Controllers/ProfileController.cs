using IronGyms.Api.DTOs;
using IronGyms.Api.Extensions;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
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
        var result = await _profileService.GetMyProfileAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto dto)
    {
        var result = await _profileService.UpdateProfileAsync(User.GetUserId(), dto);
        return Ok(new ApiResult<ProfileResponseDto> { Message = "Cập nhật hồ sơ thành công", Data = result });
    }

    [HttpPut("me/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        var result = await _profileService.UpdateAvatarAsync(User.GetUserId(), file);
        return Ok(new ApiResult<ProfileResponseDto> { Message = "Cập nhật ảnh đại diện thành công", Data = result });
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
    {
        await _profileService.ChangePasswordAsync(User.GetUserId(), dto);
        return Ok(new { message = "Đổi mật khẩu thành công, vui lòng đăng nhập lại trên các thiết bị khác" });
    }
}