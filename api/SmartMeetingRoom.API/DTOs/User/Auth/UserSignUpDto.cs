using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class UserSignUpDto
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

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [MaxLength(25, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = null!;

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

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
}
