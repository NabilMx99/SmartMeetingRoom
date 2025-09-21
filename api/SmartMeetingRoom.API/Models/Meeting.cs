using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class Meeting
{
    public int MeetingId { get; set; }

    public long? ZoomMeetingId { get; set; }

    public string? ZoomJoinUrl { get; set; }

    public int FkUserId { get; set; }

    public int FkRoomId { get; set; }

    public DateTime MeetingStartTime { get; set; }

    public DateTime MeetingEndTime { get; set; }

    public string? MeetingAgenda { get; set; }

    public string MeetingTitle { get; set; } = null!;

    public string MeetingStatus { get; set; } = null!;

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual Room FkRoom { get; set; } = null!;

    public virtual ApplicationUser FkUser { get; set; } = null!;

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();
}
