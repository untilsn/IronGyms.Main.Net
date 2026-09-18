namespace IronGyms.Api.DTOs;

public class MembershipPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMembershipPlanRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
}

// Admin update được cả IsActive - cách để "mở lại" 1 gói đã soft-delete nếu cần,
// tách biệt với DELETE (chỉ có 1 việc duy nhất là tắt gói).
public class UpdateMembershipPlanRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
}