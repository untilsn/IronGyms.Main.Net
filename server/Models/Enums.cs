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
    Cancelled
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