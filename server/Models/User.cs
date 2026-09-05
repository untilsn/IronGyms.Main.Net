namespace IronGyms.Api.Models;

// User = danh tính đăng nhập, dùng chung cho cả 4 role, phân biệt bằng Role.
// Login member (trang client) và login admin (trang quản trị) dùng CHUNG bảng này:
//  - Trang admin: chặn nếu Role == Member.
//  - Trang client: chỉ chấp nhận Role == Member.
// Xử lý ở tầng API, không cần tách bảng User riêng.
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;

    // Null khi user đăng ký qua Google OAuth, không có mật khẩu local
    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; }

    public string? GoogleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Profile? Profile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}