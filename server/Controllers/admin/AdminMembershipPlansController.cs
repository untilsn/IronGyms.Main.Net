using IronGyms.Api.DTOs;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/membership-plans")]
[Authorize(Roles = "Admin")]
public class AdminMembershipPlansController : ControllerBase
{
    private readonly IMembershipPlanService _service;

    public AdminMembershipPlansController(IMembershipPlanService service)
    {
        _service = service;
    }

    // ?isActive=true|false để lọc, bỏ trống thì lấy tất cả kể cả gói đã tắt.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
    {
        var result = await _service.GetAllAsync(isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, includeInactive: true);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMembershipPlanRequestDto dto)
    {
        var result = await _service.CreateAsync(dto);
        var body = new ApiResult<MembershipPlanResponseDto> { Message = "Tạo gói tập thành công", Data = result };
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, body);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMembershipPlanRequestDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(new ApiResult<MembershipPlanResponseDto> { Message = "Cập nhật gói tập thành công", Data = result });
    }

    // Soft delete - chỉ tắt IsActive, không xoá row thật.
    // Trả 200 kèm message thay vì 204 No Content, vì client cần message để toast xác nhận.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.SoftDeleteAsync(id);
        return Ok(new { message = "Đã tắt gói tập" });
    }
}