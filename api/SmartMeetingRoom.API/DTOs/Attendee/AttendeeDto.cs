namespace SmartMeetingRoom.API.DTOs.Attendee;

using SmartMeetingRoom.API.DTOs.User;

public class AttendeeDto
{
    public int AttendeeId { get; set; }

    public UserDto User { get; set; } = null!;

    public string AttendeeStatus { get; set; } = null!;
}