using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class Feature
{
    public byte FeatureId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string? FeatureDescription { get; set; }

    public virtual ICollection<RoomFeature> RoomFeatures { get; set; } = new List<RoomFeature>();
}
