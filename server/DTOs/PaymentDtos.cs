using IronGyms.Api.Models;

namespace IronGyms.Api.DTOs;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public PaymentFor For { get; set; }
    public Guid? MemberMembershipId { get; set; }
    public Guid? OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}