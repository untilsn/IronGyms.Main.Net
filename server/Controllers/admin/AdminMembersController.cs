using IronGyms.Api.DTOs;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/members")]
[Authorize(Roles = "Admin")]
public class AdminMembersController : ControllerBase
{
    private readonly IMemberService _service;

    public AdminMembersController(IMemberService service)
    {
        _service = service;
    }

    // ?search=tên hoặc email, ?isActive=true|false - cả 2 đều optional
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var result = await _service.GetAllAsync(search, isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    // Khoá/mở khoá tài khoản - không xoá member thật, chỉ đổi IsActive
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] UpdateMemberStatusRequestDto dto)
    {
        await _service.SetActiveStatusAsync(id, dto.IsActive);
        return Ok(new { message = dto.IsActive ? "Đã mở khoá tài khoản" : "Đã khoá tài khoản" });
    }
}