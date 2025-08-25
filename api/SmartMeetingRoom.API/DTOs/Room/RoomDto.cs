using SmartMeetingRoom.API.DTOs.Feature;

namespace SmartMeetingRoom.API.DTOs.Room;

public class RoomDto
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public string RoomLocation { get; set; } = null!;

    public byte RoomCapacity { get; set; }

    public bool IsAvailable { get; set; }

    public ICollection<FeatureDto> Features { get; set; } = new List<FeatureDto>();
}
