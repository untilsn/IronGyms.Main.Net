using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> ConfirmCodPaymentAsync(Guid paymentId);
    Task<List<PaymentResponseDto>> GetAllAsync(PaymentStatus? status);
}

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;

    public PaymentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PaymentResponseDto> ConfirmCodPaymentAsync(Guid paymentId)
    {
        var payment = await _db.Payments
            .Include(p => p.MemberMembership)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null)
            throw ApiException.NotFound("Không tìm thấy giao dịch thanh toán");

        if (payment.Method != PaymentMethod.Cod)
            throw ApiException.BadRequest("Chỉ xác nhận thủ công được cho phương thức COD");

        if (payment.Status != PaymentStatus.Pending)
            throw ApiException.BadRequest("Giao dịch này đã được xử lý trước đó");

        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;

        // Kích hoạt đúng tài nguyên tương ứng dựa vào Payment.For - mở rộng thêm case Order
        // khi module Shop được code, chưa nằm trong scope hiện tại.
        switch (payment.For)
        {
            case PaymentFor.Membership when payment.MemberMembership != null:
                payment.MemberMembership.Status = MembershipStatus.Active;
                break;
            case PaymentFor.Order:
                throw ApiException.BadRequest("Xác nhận thanh toán cho Order chưa được hỗ trợ");
        }

        await _db.SaveChangesAsync();
        return MapToDto(payment);
    }

    public async Task<List<PaymentResponseDto>> GetAllAsync(PaymentStatus? status)
    {
        var query = _db.Payments.AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var payments = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return payments.Select(MapToDto).ToList();
    }

    private static PaymentResponseDto MapToDto(Payment p) => new()
    {
        Id = p.Id,
        For = p.For,
        MemberMembershipId = p.MemberMembershipId,
        OrderId = p.OrderId,
        Amount = p.Amount,
        Method = p.Method,
        Status = p.Status,
        PaidAt = p.PaidAt,
        CreatedAt = p.CreatedAt
    };
}