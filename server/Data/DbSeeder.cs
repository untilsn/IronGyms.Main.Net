using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Data;

public static class DbSeeder
{
    private static readonly string[] SampleNames =
    {
        "Nguyễn Văn An", "Trần Thị Bình", "Lê Văn Cường", "Phạm Thị Dung", "Hoàng Văn Em",
        "Vũ Thị Phương", "Đặng Văn Giang", "Bùi Thị Hoa", "Đỗ Văn Inh", "Ngô Thị Kim",
        "Dương Văn Long", "Lý Thị Mai", "Trịnh Văn Nam", "Đinh Thị Oanh", "Phan Văn Phúc",
        "Vương Thị Quyên", "Tô Văn Sơn", "Lâm Thị Thu", "Chu Văn Uy", "Mai Thị Vân"
    };

    // Mật khẩu chung cho toàn bộ member seed, chỉ dùng để test - KHÔNG seed data này ở môi trường Production.
    private const string DefaultPassword = "Member@123";

    public static async Task SeedMembersAsync(AppDbContext db)
    {
        var existingCount = await db.Users.CountAsync(u => u.Role == UserRole.Member);
        if (existingCount >= SampleNames.Length)
            return; // đã seed đủ rồi - tránh tạo trùng mỗi lần restart server lúc dev

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
        var random = new Random();

        for (var i = 0; i < SampleNames.Length; i++)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = $"member{i + 1}@irongyms.test",
                PasswordHash = passwordHash,
                Role = UserRole.Member,
                IsActive = true,
                IsEmailVerified = true
            };

            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FullName = SampleNames[i],
                PhoneNumber = $"09{random.Next(10_000_000, 99_999_999)}",
                Gender = i % 2 == 0 ? Models.Gender.Male : Models.Gender.Female,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-random.Next(18, 45)).AddDays(-random.Next(0, 365)))
            };

            var memberDetail = new MemberDetail { ProfileId = profile.Id };

            db.Users.Add(user);
            db.Profiles.Add(profile);
            db.MemberDetails.Add(memberDetail);
        }

        await db.SaveChangesAsync();
    }
}