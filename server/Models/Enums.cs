namespace IronGyms.Api.Models;

public enum UserRole
{
    Member,
    Trainer,
    Staff,
    Admin
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum MembershipStatus
{
    Active,
    Expired,
    Cancelled,
    // Thêm ở CUỐI enum, không chen vào giữa - tránh đổi giá trị số nguyên của các trạng thái
    // đã tồn tại (EF Core lưu enum dạng int mặc định, chen vào giữa sẽ làm sai lệch data cũ).
    PendingPayment
}

public enum CheckInMethod
{
    QrCode,
    Manual
}

public enum PaymentMethod
{
    Cod,
    PayPal
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}

// Payment dùng chung cho 2 mục đích: mua gói tập hoặc mua đồ trong shop
public enum PaymentFor
{
    Membership,
    Order
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Ready,      // sẵn sàng lấy tại quầy, gym bán tại chỗ nên không cần trạng thái ship
    Completed,
    Cancelled
}