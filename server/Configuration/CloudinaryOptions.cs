namespace IronGyms.Api.Configuration;

// Bind từ appsettings.json:
// "Cloudinary": { "CloudName": "...", "ApiKey": "...", "ApiSecret": "..." }
// ApiSecret để trong User Secrets lúc dev, biến môi trường lúc deploy - không commit vào appsettings.json.
public class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
}