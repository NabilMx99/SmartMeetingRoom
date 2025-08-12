using Microsoft.AspNetCore.Identity;

namespace SmartMeetingRoom.API.Models;

public class ApplicationUser : IdentityUser<int>
{
    public int FkRoleId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool IsOnline { get; set; }

    public virtual ApplicationRole FkRole { get; set; } = null!;

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}