using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class UserLoginDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Password")]
    [MinLength(8, ErrorMessage = "{0} must be at least 8 characters.")]
    [MaxLength(128, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
