using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.User;

public class UserProfileDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "First Name")]
    [MinLength(2, ErrorMessage = "{0} must be at least {1} characters.")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Last Name")]
    [MinLength(2, ErrorMessage = "{0} must be at least {1} characters.")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [MaxLength(25, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}

