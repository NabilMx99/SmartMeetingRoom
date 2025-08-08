namespace SmartMeetingRoom.API.DTOs.Role;

public class RoleDto
{
    public byte RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }
}