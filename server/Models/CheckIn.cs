namespace IronGyms.Api.Models;

// Được tạo khi staff/máy quét quét QR hợp lệ của member (hoặc check-in tay).
public class CheckIn
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; } // FK -> MemberDetail.ProfileId
    public CheckInMethod Method { get; set; }

    // Chỉ có giá trị khi Method = Manual, để biết staff nào đã check-in hộ
    public Guid? StaffId { get; set; } // FK -> StaffDetail.ProfileId

    public DateTime CheckedInAt { get; set; } = DateTime.UtcNow;

    public MemberDetail Member { get; set; } = null!;
    public StaffDetail? Staff { get; set; }
}