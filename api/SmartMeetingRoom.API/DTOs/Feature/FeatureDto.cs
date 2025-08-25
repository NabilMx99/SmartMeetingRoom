namespace SmartMeetingRoom.API.DTOs.Feature;

public class FeatureDto
{
    public byte FeatureId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string? FeatureDescription { get; set; }
}
