using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "New Password")]
    [MinLength(8, ErrorMessage = "{0} must be at least {1} characters.")]
    [MaxLength(128, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Confirm Password")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    [MaxLength(128, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Token")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Email")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;
}