using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class ActionItem
{
    public int ActionItemId { get; set; }

    public int FkMeetingMinutesId { get; set; }

    public int FkUserId { get; set; }

    public DateTime ActionItemDueDate { get; set; }

    public string ActionItemDescription { get; set; } = null!;

    public string ActionItemStatus { get; set; } = null!;

    public virtual MeetingMinute FkMeetingMinutes { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
