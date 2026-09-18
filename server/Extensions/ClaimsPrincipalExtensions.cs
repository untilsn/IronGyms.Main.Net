using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IronGyms.Api.Exceptions;

namespace IronGyms.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    // "sub" trong JWT thường được ASP.NET Core tự map thành ClaimTypes.NameIdentifier,
    // nhưng thử cả 2 kiểu cho chắc (tuỳ cấu hình DefaultMapInboundClaims có bật hay không).
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (value == null || !Guid.TryParse(value, out var userId))
            throw ApiException.Unauthorized("Token không chứa UserId hợp lệ");

        return userId;
    }
}