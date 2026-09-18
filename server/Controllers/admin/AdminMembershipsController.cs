using IronGyms.Api.Models;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers.Admin;

// Staff cũng cần xem để đối chiếu lúc khách tới quầy thanh toán, không chỉ riêng Admin.
[ApiController]
[Route("api/admin/memberships")]
[Authorize(Roles = "Admin,Staff")]
public class AdminMembershipsController : ControllerBase
{
    private readonly IMemberMembershipService _service;

    public AdminMembershipsController(IMemberMembershipService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MembershipStatus? status)
    {
        var result = await _service.GetAllAsync(status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }
}