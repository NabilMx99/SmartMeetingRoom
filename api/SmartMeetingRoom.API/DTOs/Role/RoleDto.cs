namespace SmartMeetingRoom.API.DTOs.Role;

public class RoleDto
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }
}