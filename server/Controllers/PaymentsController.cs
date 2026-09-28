using IronGyms.Api.DTOs;
using IronGyms.Api.Extensions;
using IronGyms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IronGyms.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Member")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    // Member tự gọi sau khi PayPal redirect về ReturnUrl báo đã duyệt thanh toán.
    [HttpPut("{id:guid}/capture-paypal")]
    public async Task<IActionResult> CapturePayPal(Guid id)
    {
        var result = await _service.CapturePayPalPaymentAsync(User.GetUserId(), id);
        return Ok(new ApiResult<PaymentResponseDto> { Message = "Thanh toán PayPal thành công", Data = result });
    }
}