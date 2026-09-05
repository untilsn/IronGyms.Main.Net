namespace IronGyms.Api.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; } // FK -> MemberDetail.ProfileId
    public string OrderCode { get; set; } = null!; // vd "ORD-20260904-0001"
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Snapshot tổng tiền = sum(OrderItem.Subtotal) tại thời điểm đặt hàng
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public MemberDetail Member { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}