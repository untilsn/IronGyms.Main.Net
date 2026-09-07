using System.Security.Claims;

namespace IronGyms.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    // "sub" trong JWT được ASP.NET Core tự map thành ClaimTypes.NameIdentifier khi đọc token.
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Token không chứa UserId");

        return Guid.Parse(idClaim);
    }
}