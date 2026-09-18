using IronGyms.Api.Data;
using IronGyms.Api.DTOs;
using IronGyms.Api.Exceptions;
using IronGyms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IronGyms.Api.Services;

public interface IMembershipPlanService
{
    // isActive = null -> lấy tất cả (chỉ Admin dùng); true/false -> lọc đúng trạng thái đó.
    Task<List<MembershipPlanResponseDto>> GetAllAsync(bool? isActive);

    // includeInactive = false -> Client gọi, ẩn gói đã tắt (trả 404 luôn, không lộ gói đó tồn tại).
    Task<MembershipPlanResponseDto> GetByIdAsync(Guid id, bool includeInactive);

    Task<MembershipPlanResponseDto> CreateAsync(CreateMembershipPlanRequestDto dto);
    Task<MembershipPlanResponseDto> UpdateAsync(Guid id, UpdateMembershipPlanRequestDto dto);
    Task SoftDeleteAsync(Guid id);
}

public class MembershipPlanService : IMembershipPlanService
{
    private readonly AppDbContext _db;

    public MembershipPlanService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MembershipPlanResponseDto>> GetAllAsync(bool? isActive)
    {
        var query = _db.MembershipPlans.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        // Lấy entity về trước rồi mới map bằng LINQ to Objects - EF Core không thể dịch
        // lời gọi hàm C# tuỳ ý (MapToDto) thành SQL nếu gọi ngay trong .Select() ở tầng query.
        var plans = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return plans.Select(MapToDto).ToList();
    }

    public async Task<MembershipPlanResponseDto> GetByIdAsync(Guid id, bool includeInactive)
    {
        var plan = await _db.MembershipPlans.FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null || (!includeInactive && !plan.IsActive))
            throw ApiException.NotFound("Không tìm thấy gói tập");

        return MapToDto(plan);
    }

    public async Task<MembershipPlanResponseDto> CreateAsync(CreateMembershipPlanRequestDto dto)
    {
        var plan = new MembershipPlan
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DurationDays = dto.DurationDays,
            IsActive = true
        };

        _db.MembershipPlans.Add(plan);
        await _db.SaveChangesAsync();

        return MapToDto(plan);
    }

    public async Task<MembershipPlanResponseDto> UpdateAsync(Guid id, UpdateMembershipPlanRequestDto dto)
    {
        var plan = await _db.MembershipPlans.FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null)
            throw ApiException.NotFound("Không tìm thấy gói tập");

        plan.Name = dto.Name;
        plan.Description = dto.Description;
        plan.Price = dto.Price;
        plan.DurationDays = dto.DurationDays;
        plan.IsActive = dto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(plan);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var plan = await _db.MembershipPlans.FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null)
            throw ApiException.NotFound("Không tìm thấy gói tập");

        // Soft delete - không xoá row thật, vì MemberMembership cũ vẫn cần trỏ về đúng gói này
        // để hiển thị lịch sử mua hàng chính xác.
        plan.IsActive = false;
        plan.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private static MembershipPlanResponseDto MapToDto(MembershipPlan plan)
    {
        return new MembershipPlanResponseDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}