namespace IronGyms.Api.Models;

public class MemberMembership
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; } // FK -> MemberDetail.ProfileId
    public Guid MembershipPlanId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipStatus Status { get; set; }

    // Snapshot giá tại thời điểm mua, vì giá của MembershipPlan có thể đổi sau này
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MemberDetail Member { get; set; } = null!;
    public MembershipPlan MembershipPlan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}