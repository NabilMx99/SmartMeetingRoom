using SmartMeetingRoom.API.DTOs.Role;

namespace SmartMeetingRoom.API.DTOs.User;

public class UserDto
{
    public int UserId { get; set; }

    public RoleDto Role { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsOnline { get; set; }
}

