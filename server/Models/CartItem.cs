namespace IronGyms.Api.Models;

// Không snapshot giá ở đây - giỏ hàng là tạm thời, giá luôn lấy trực tiếp từ Product.Price.
public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}