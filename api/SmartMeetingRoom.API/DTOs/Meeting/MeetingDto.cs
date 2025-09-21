using SmartMeetingRoom.API.DTOs.User;
using SmartMeetingRoom.API.DTOs.Room;
using SmartMeetingRoom.API.DTOs.Attendee;

namespace SmartMeetingRoom.API.DTOs.Meeting;

public class MeetingDto
{
    public int MeetingId { get; set; }

    public long? ZoomMeetingId { get; set; }

    public UserDto Organizer { get; set; } = null!;

    public RoomDto Room { get; set; } = null!;

    public DateTime MeetingStartTime { get; set; }

    public DateTime MeetingEndTime { get; set; }

    public string MeetingTitle { get; set; } = null!;

    public string? MeetingAgenda { get; set; }

    public string MeetingStatus { get; set; } = null!;

    public string? ZoomJoinUrl { get; set; }

    public ICollection<AttendeeDto> Attendees { get; set; } = new List<AttendeeDto>();
}