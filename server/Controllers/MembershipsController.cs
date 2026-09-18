using IronGyms.Api.DTOs;
using IronGyms.Api.Extensions;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

[ApiController]
[Route("api/memberships")]
[Authorize(Roles = "Member")]
public class MembershipsController : ControllerBase
{
    private readonly IMemberMembershipService _service;

    public MembershipsController(IMemberMembershipService service)
    {
        _service = service;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseMembershipRequestDto dto)
    {
        var result = await _service.PurchaseAsync(User.GetUserId(), dto);
        return Ok(new ApiResult<MemberMembershipResponseDto>
        {
            Message = "Đăng ký gói tập thành công, vui lòng thanh toán tại quầy để kích hoạt",
            Data = result
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy()
    {
        var result = await _service.GetMyMembershipsAsync(User.GetUserId());
        return Ok(result);
    }
}