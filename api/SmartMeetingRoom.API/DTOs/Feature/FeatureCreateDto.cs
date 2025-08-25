using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Feature;

public class FeatureCreateDto
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Feature Name")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string FeatureName { get; set; } = null!;

    [Display(Name = "Feature Description")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? FeatureDescription { get; set; }
}
