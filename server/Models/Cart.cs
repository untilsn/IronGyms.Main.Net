namespace IronGyms.Api.Models;

// Mỗi Member có đúng 1 giỏ hàng sống - nhớ unique index trên MemberId khi config EF.
public class Cart
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; } // FK -> MemberDetail.ProfileId
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public MemberDetail Member { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}