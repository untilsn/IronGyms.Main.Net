namespace IronGyms.Api.Models;

// Quan hệ 1-1 với Profile bằng shared primary key (ProfileId vừa là PK vừa là FK).
// Config trong OnModelCreating:
//   modelBuilder.Entity<MemberDetail>().HasKey(d => d.ProfileId);
//   modelBuilder.Entity<MemberDetail>()
//       .HasOne(d => d.Profile).WithOne(p => p.MemberDetail)
//       .HasForeignKey<MemberDetail>(d => d.ProfileId);
public class MemberDetail
{
    public Guid ProfileId { get; set; }

    // QR check-in - sinh lại (regenerate) mỗi lần member mở trang check-in,
    // sống ngắn hạn (60-90s) để tránh bị chụp màn hình dùng lại nhiều lần.
    public string? QrToken { get; set; }
    public DateTime? QrTokenExpiresAt { get; set; }

    public Profile Profile { get; set; } = null!;
    public ICollection<MemberMembership> Memberships { get; set; } = new List<MemberMembership>();
    public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public Cart? Cart { get; set; }
}