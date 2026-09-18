using IronGyms.Api.Models;

namespace IronGyms.Api.DTOs;

public class MemberListItemDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MemberDetailDto : MemberListItemDto
{
    public DateOnly? DateOfBirth { get; set; }
    public string? AddressLine { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateMemberStatusRequestDto
{
    public bool IsActive { get; set; }
}