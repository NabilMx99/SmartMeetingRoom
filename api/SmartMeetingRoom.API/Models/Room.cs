using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public string RoomLocation { get; set; } = null!;

    public byte RoomCapacity { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<RoomFeature> RoomFeatures { get; set; } = new List<RoomFeature>();
}
