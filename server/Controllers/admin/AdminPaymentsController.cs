using IronGyms.Api.DTOs;
using IronGyms.Api.Models;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/payments")]
[Authorize(Roles = "Admin,Staff")]
public class AdminPaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public AdminPaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaymentStatus? status)
    {
        var result = await _service.GetAllAsync(status);
        return Ok(result);
    }

    // Staff bấm nút này tại quầy sau khi thu tiền mặt từ khách.
    [HttpPut("{id:guid}/confirm-cod")]
    public async Task<IActionResult> ConfirmCod(Guid id)
    {
        var result = await _service.ConfirmCodPaymentAsync(id);
        return Ok(new ApiResult<PaymentResponseDto> { Message = "Đã xác nhận thanh toán COD", Data = result });
    }
}