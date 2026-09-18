using IronGyms.Api.Models;

namespace IronGyms.Api.DTOs;

public class PurchaseMembershipRequestDto
{
    public Guid MembershipPlanId { get; set; }
}

public class PaymentSummaryDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class MemberMembershipResponseDto
{
    public Guid Id { get; set; }
    public Guid MembershipPlanId { get; set; }
    public string MembershipPlanName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public MembershipStatus Status { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    // Payment gần nhất gắn với gói này - null chỉ xảy ra nếu dữ liệu bất thường,
    // vì luồng mua luôn tạo Payment cùng lúc với MemberMembership.
    public PaymentSummaryDto? Payment { get; set; }
}