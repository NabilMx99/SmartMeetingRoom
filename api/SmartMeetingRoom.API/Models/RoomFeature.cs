using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class RoomFeature
{
    public int RoomFeatureId { get; set; }

    public int FkRoomId { get; set; }

    public byte FkFeatureId { get; set; }

    public virtual Feature FkFeature { get; set; } = null!;

    public virtual Room FkRoom { get; set; } = null!;
}
