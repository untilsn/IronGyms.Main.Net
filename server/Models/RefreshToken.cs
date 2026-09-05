namespace IronGyms.Api.Models;

// Chỉ lưu HASH của refresh token (vd SHA-256), không lưu raw token trong DB.
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }
    // Khi refresh, token cũ bị revoke và trỏ sang token mới thay thế (rotation)
    public string? ReplacedByTokenHash { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; set; } = null!;
}