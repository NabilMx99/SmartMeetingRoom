using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public int FkRoleId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsOnline { get; set; }

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual Role FkRole { get; set; } = null!;

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}
