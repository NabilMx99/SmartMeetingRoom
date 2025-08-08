using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Role;

public class RoleUpdateDto
{
    [Display(Name = "Role Description")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed 255 characters.")]
    public string? RoleDescription { get; set; }
}
