using IronGyms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

// Không gắn [Authorize] - khách xem giá gói tập trước khi đăng ký cũng hợp lý.
[ApiController]
[Route("api/membership-plans")]
public class MembershipPlansController : ControllerBase
{
    private readonly IMembershipPlanService _service;

    public MembershipPlansController(IMembershipPlanService service)
    {
        _service = service;
    }

    // Luôn chỉ trả gói đang bán (IsActive = true) - Client không cần biết gói nào đã ẩn.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(isActive: true);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, includeInactive: false);
        return Ok(result);
    }
}