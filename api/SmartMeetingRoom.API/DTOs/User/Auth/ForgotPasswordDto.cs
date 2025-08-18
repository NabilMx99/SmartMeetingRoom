using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
}
