using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs;

public class RoleCreateDto
{
    [Required]
    [Display(Name = "Role Name")]
    [MaxLength(20, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [RegularExpression("^(Admin|Employee|Guest)$", ErrorMessage = "{0} must be Admin, Employee or Guest.")]
    public string RoleName { get; set; } = null!;

    [Display(Name = "Role Description")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed 255 characters.")]
    public string? RoleDescription { get; set; }
}

