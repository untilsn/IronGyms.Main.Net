namespace IronGyms.Api.Models;

// Shared primary key với Profile - config tương tự MemberDetail (xem comment ở đó).
public class TrainerDetail
{
    public Guid ProfileId { get; set; }

    public string? Specialization { get; set; } // vd: "Yoga, Gym cơ bản"
    public string? Bio { get; set; }

    public Profile Profile { get; set; } = null!;
}