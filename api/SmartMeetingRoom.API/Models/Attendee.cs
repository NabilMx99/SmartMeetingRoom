using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class Attendee
{
    public int AttendeeId { get; set; }

    public int FkUserId { get; set; }

    public int FkMeetingId { get; set; }

    public string AttendeeStatus { get; set; } = null!;

    public virtual Meeting FkMeeting { get; set; } = null!;

    public virtual ApplicationUser FkUser { get; set; } = null!;
}
