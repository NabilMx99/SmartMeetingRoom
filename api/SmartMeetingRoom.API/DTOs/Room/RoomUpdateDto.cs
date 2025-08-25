using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Room;

public class RoomUpdateDto
{
    [Display(Name = "Room Name")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? RoomName { get; set; }

    [Display(Name = "Room Location")]
    [MaxLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? RoomLocation { get; set; }

    [Display(Name = "Room Capacity")]
    [Range(1, 255, ErrorMessage = "{0} must be between {1} and {2}.")]
    public byte? RoomCapacity { get; set; }

    [Display(Name = "Is Available")]
    public bool? IsAvailable { get; set; }

    [Display(Name = "Features")]
    public ICollection<byte>? FeatureIds { get; set; }
}