namespace IronGyms.Api.Models;

// Shared primary key với Profile - config tương tự MemberDetail (xem comment ở đó).
public class StaffDetail
{
    public Guid ProfileId { get; set; }

    public DateOnly? HireDate { get; set; }

    public Profile Profile { get; set; } = null!;
    public ICollection<CheckIn> ManualCheckIns { get; set; } = new List<CheckIn>();
}