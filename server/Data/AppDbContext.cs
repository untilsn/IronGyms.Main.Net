using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<MemberDetail> MemberDetails => Set<MemberDetail>();
    public DbSet<TrainerDetail> TrainerDetails => Set<TrainerDetail>();
    public DbSet<StaffDetail> StaffDetails => Set<StaffDetail>();

    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<MemberMembership> MemberMemberships => Set<MemberMembership>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- User ----------
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.GoogleId).IsUnique().HasFilter("\"GoogleId\" IS NOT NULL");

            e.HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- RefreshToken ----------
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Profile + shared-primary-key Details ----------
        modelBuilder.Entity<MemberDetail>(e =>
        {
            e.HasKey(d => d.ProfileId);
            e.HasOne(d => d.Profile)
                .WithOne(p => p.MemberDetail)
                .HasForeignKey<MemberDetail>(d => d.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TrainerDetail>(e =>
        {
            e.HasKey(d => d.ProfileId);
            e.HasOne(d => d.Profile)
                .WithOne(p => p.TrainerDetail)
                .HasForeignKey<TrainerDetail>(d => d.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StaffDetail>(e =>
        {
            e.HasKey(d => d.ProfileId);
            e.HasOne(d => d.Profile)
                .WithOne(p => p.StaffDetail)
                .HasForeignKey<StaffDetail>(d => d.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- MemberMembership ----------
        modelBuilder.Entity<MemberMembership>(e =>
        {
            e.HasOne(m => m.Member)
                .WithMany(md => md.Memberships)
                .HasForeignKey(m => m.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.MembershipPlan)
                .WithMany(p => p.MemberMemberships)
                .HasForeignKey(m => m.MembershipPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- CheckIn ----------
        modelBuilder.Entity<CheckIn>(e =>
        {
            e.HasOne(c => c.Member)
                .WithMany(md => md.CheckIns)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.Staff)
                .WithMany(sd => sd.ManualCheckIns)
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ---------- Shop ----------
        modelBuilder.Entity<Product>(e =>
        {
            e.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cart>(e =>
        {
            e.HasIndex(c => c.MemberId).IsUnique();
            e.HasOne(c => c.Member)
                .WithOne(md => md.Cart)
                .HasForeignKey<Cart>(c => c.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(e =>
        {
            e.HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();
            e.HasOne(ci => ci.Cart).WithMany(c => c.Items).HasForeignKey(ci => ci.CartId);
            e.HasOne(ci => ci.Product).WithMany(p => p.CartItems).HasForeignKey(ci => ci.ProductId);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasIndex(o => o.OrderCode).IsUnique();
            e.HasOne(o => o.Member)
                .WithMany(md => md.Orders)
                .HasForeignKey(o => o.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            e.HasOne(oi => oi.Product).WithMany(p => p.OrderItems).HasForeignKey(oi => oi.ProductId);
        });

        // ---------- Payment (dùng chung Membership/Order) ----------
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasOne(p => p.MemberMembership)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberMembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t => t.HasCheckConstraint(
                "CK_Payment_ExactlyOneTarget",
                "(\"MemberMembershipId\" IS NOT NULL AND \"OrderId\" IS NULL) OR " +
                "(\"MemberMembershipId\" IS NULL AND \"OrderId\" IS NOT NULL)"));
        });
    }
}