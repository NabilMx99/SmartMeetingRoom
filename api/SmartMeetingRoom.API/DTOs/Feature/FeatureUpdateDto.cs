using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Feature;

public class FeatureUpdateDto
{
    [Display(Name = "Feature Name")]
    [MaxLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? FeatureName { get; set; }

    [Display(Name = "Feature Description")]
    [MaxLength(255, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? FeatureDescription { get; set; }
}