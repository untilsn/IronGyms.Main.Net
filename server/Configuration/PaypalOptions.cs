namespace IronGyms.Api.Configuration;

// Bind từ appsettings.json:
// "PayPal": { "ClientId": "...", "ClientSecret": "...", "BaseUrl": "https://api-m.sandbox.paypal.com",
//             "ReturnUrl": "...", "CancelUrl": "..." }
// ClientSecret để trong User Secrets lúc dev - không commit vào appsettings.json.
// Đổi BaseUrl thành https://api-m.paypal.com khi lên Production (hiện đang trỏ Sandbox để test).
public class PayPalOptions
{
    public const string SectionName = "PayPal";

    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";

    // Trang phía client mà PayPal redirect về sau khi khách duyệt/huỷ thanh toán.
    public string ReturnUrl { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
}