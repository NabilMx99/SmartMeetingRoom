using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class MeetingMinute
{
    public int MeetingMinutesId { get; set; }

    public int FkMeetingId { get; set; }

    public int FkUserId { get; set; }

    public string MeetingSummary { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();

    public virtual Meeting FkMeeting { get; set; } = null!;

    public virtual ApplicationUser FkUser { get; set; } = null!;
}
