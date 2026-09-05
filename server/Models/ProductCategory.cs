namespace IronGyms.Api.Models;

public class ProductCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!; // vd "Quần áo", "Dụng cụ tập"
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}