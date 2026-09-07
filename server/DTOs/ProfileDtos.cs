using IronGyms.Api.Models;
using System.ComponentModel.DataAnnotations;


namespace IronGyms.Api.DTOs;

public class ProfileResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }

    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AddressLine { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? AvatarUrl { get; set; }

    // Chỉ có giá trị tương ứng với Role, còn lại null
    public string? Specialization { get; set; } // Trainer
    public string? Bio { get; set; }             // Trainer
    public DateTime? HireDate { get; set; }      // Staff
}

public class UpdateProfileRequestDto
{
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AddressLine { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
}

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    public string ConfirmPassword { get; set; } = null!;
}