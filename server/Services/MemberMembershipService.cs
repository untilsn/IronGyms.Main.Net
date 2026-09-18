using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IMemberMembershipService
{
    Task<MemberMembershipResponseDto> PurchaseAsync(Guid userId, PurchaseMembershipRequestDto dto);
    Task<List<MemberMembershipResponseDto>> GetMyMembershipsAsync(Guid userId);
    Task<List<MemberMembershipResponseDto>> GetAllAsync(MembershipStatus? status);
    Task<MemberMembershipResponseDto> GetByIdAsync(Guid id);
}

public class MemberMembershipService : IMemberMembershipService
{
    private readonly AppDbContext _db;

    public MemberMembershipService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MemberMembershipResponseDto> PurchaseAsync(Guid userId, PurchaseMembershipRequestDto dto)
    {
        var memberId = await ResolveMemberIdAsync(userId);

        var plan = await _db.MembershipPlans.FirstOrDefaultAsync(p => p.Id == dto.MembershipPlanId);
        if (plan == null || !plan.IsActive)
            throw ApiException.NotFound("Không tìm thấy gói tập");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var membership = new MemberMembership
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            MembershipPlanId = plan.Id,
            StartDate = today,
            EndDate = today.AddDays(plan.DurationDays),
            Status = MembershipStatus.PendingPayment,
            Price = plan.Price // snapshot giá tại thời điểm mua, không lấy giá plan hiện tại về sau
        };

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            For = PaymentFor.Membership,
            MemberMembershipId = membership.Id,
            Amount = plan.Price,
            Method = PaymentMethod.Cod,
            Status = PaymentStatus.Pending
        };

        _db.MemberMemberships.Add(membership);
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        return ToDto(membership, plan, payment);
    }

    public async Task<List<MemberMembershipResponseDto>> GetMyMembershipsAsync(Guid userId)
    {
        var memberId = await ResolveMemberIdAsync(userId);

        var memberships = await _db.MemberMemberships
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .Where(m => m.MemberId == memberId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return memberships.Select(ToDtoFromLoaded).ToList();
    }

    public async Task<List<MemberMembershipResponseDto>> GetAllAsync(MembershipStatus? status)
    {
        var query = _db.MemberMemberships
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        var memberships = await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
        return memberships.Select(ToDtoFromLoaded).ToList();
    }

    public async Task<MemberMembershipResponseDto> GetByIdAsync(Guid id)
    {
        var membership = await _db.MemberMemberships
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
            throw ApiException.NotFound("Không tìm thấy gói đã mua");

        return ToDtoFromLoaded(membership);
    }

    // User.Id (từ JWT) khác với MemberDetail.ProfileId (khoá của MemberMembership.MemberId) -
    // phải đi qua Profile để map đúng, vì Profile.UserId là cầu nối duy nhất giữa 2 bên.
    private async Task<Guid> ResolveMemberIdAsync(Guid userId)
    {
        var memberDetail = await _db.MemberDetails
            .Include(md => md.Profile)
            .FirstOrDefaultAsync(md => md.Profile.UserId == userId);

        if (memberDetail == null)
            throw ApiException.Forbidden("Chỉ tài khoản Member mới thực hiện được hành động này");

        return memberDetail.ProfileId;
    }

    private static MemberMembershipResponseDto ToDtoFromLoaded(MemberMembership m)
    {
        var latestPayment = m.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        return ToDto(m, m.MembershipPlan, latestPayment);
    }

    private static MemberMembershipResponseDto ToDto(MemberMembership m, MembershipPlan plan, Payment? payment)
    {
        return new MemberMembershipResponseDto
        {
            Id = m.Id,
            MembershipPlanId = m.MembershipPlanId,
            MembershipPlanName = plan.Name,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Status = m.Status,
            Price = m.Price,
            CreatedAt = m.CreatedAt,
            Payment = payment == null ? null : new PaymentSummaryDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                PaidAt = payment.PaidAt
            }
        };
    }
}