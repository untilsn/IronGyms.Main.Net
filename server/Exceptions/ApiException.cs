using Microsoft.AspNetCore.Http;

namespace IronGyms.Api.Exceptions;

// Exception dùng CHUNG cho mọi lỗi nghiệp vụ cần trả đúng HTTP status code về client -
// không riêng gì Auth. Tên cũ "AuthException" dễ khiến người đọc tưởng chỉ dùng cho đăng nhập,
// trong khi thực tế ProfileService, MembershipPlanService, CloudinaryService... đều dùng nó.
//
// Dùng qua các factory method bên dưới thay vì gọi constructor trực tiếp với số status code
// viết tay (vd "404") - vừa dễ đọc, vừa tránh gõ nhầm số.
public class ApiException : Exception
{
    public int StatusCode { get; }

    private ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public static ApiException BadRequest(string message) => new(message, StatusCodes.Status400BadRequest);
    public static ApiException Unauthorized(string message) => new(message, StatusCodes.Status401Unauthorized);
    public static ApiException Forbidden(string message) => new(message, StatusCodes.Status403Forbidden);
    public static ApiException NotFound(string message) => new(message, StatusCodes.Status404NotFound);
    public static ApiException Conflict(string message) => new(message, StatusCodes.Status409Conflict);
    public static ApiException BadGateway(string message) => new(message, StatusCodes.Status502BadGateway);
}