using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Room;

public class RoomCreateDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Room Name")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string RoomName { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Room Location")]
    [MaxLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string RoomLocation { get; set; } = null!;

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Room Capacity")]
    [Range(1, 255, ErrorMessage = "{0} must be between {1} and {2}.")]
    public byte RoomCapacity { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Features")]
    [MinLength(1, ErrorMessage = "A room must have at least {1} feature.")]
    public ICollection<byte> FeatureIds { get; set; } = new List<byte>();
}
