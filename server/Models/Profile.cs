namespace IronGyms.Api.Models;

// Field chung cho MỌI role - Member/Trainer/Staff/Admin đều update qua đúng 1 bảng này.
// Field đặc thù riêng từng role nằm ở MemberDetail / TrainerDetail / StaffDetail
// (Admin không có bảng riêng vì không có field đặc thù nào).
public class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AddressLine { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AvatarPublicId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    // Chỉ đúng 1 trong 3 cái dưới có giá trị, tuỳ theo User.Role
    public MemberDetail? MemberDetail { get; set; }
    public TrainerDetail? TrainerDetail { get; set; }
    public StaffDetail? StaffDetail { get; set; }
}