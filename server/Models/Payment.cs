namespace IronGyms.Api.Models;

// 1 bảng Payment dùng chung cho 2 mục đích (mua gói tập / mua hàng shop) thay vì
// tách 2 bảng trùng lặp gần như toàn bộ field.
// Quy tắc: đúng 1 trong 2 FK (MemberMembershipId / OrderId) có giá trị, còn lại null.
// Thêm CHECK constraint ở OnModelCreating:
//
// modelBuilder.Entity<Payment>().ToTable(t => t.HasCheckConstraint(
//     "CK_Payment_ExactlyOneTarget",
//     "(\"MemberMembershipId\" IS NOT NULL AND \"OrderId\" IS NULL) OR " +
//     "(\"MemberMembershipId\" IS NULL AND \"OrderId\" IS NOT NULL)"));
public class Payment
{
    public Guid Id { get; set; }
    public PaymentFor For { get; set; }

    public Guid? MemberMembershipId { get; set; }
    public Guid? OrderId { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } // Cod hoặc PayPal
    public PaymentStatus Status { get; set; }

    // Chỉ có giá trị khi Method = PayPal
    public string? ProviderTransactionId { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MemberMembership? MemberMembership { get; set; }
    public Order? Order { get; set; }
}