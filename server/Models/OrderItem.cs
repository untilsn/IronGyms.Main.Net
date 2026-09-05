namespace IronGyms.Api.Models;

// Snapshot tên + giá tại thời điểm mua vì Product.Name/Price có thể đổi sau này.
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNameSnapshot { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}