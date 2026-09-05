namespace IronGyms.Api.Configuration;

// Bind từ appsettings.json:
// "Jwt": { "Issuer": "IronGyms", "Audience": "IronGymsClient", "SecretKey": "...", "AccessTokenExpiryMinutes": 15, "RefreshTokenExpiryDays": 30 }
// SecretKey để trong User Secrets lúc dev, biến môi trường lúc deploy - KHÔNG commit vào appsettings.json.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;

    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 30;
}