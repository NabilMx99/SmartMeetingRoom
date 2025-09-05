using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Attendee;

public class AttendeeUpdateDto
{
    [Display(Name = "Attendee Status")]
    [Required(ErrorMessage = "{0} is required.")]
    [RegularExpression("^(Invited|Accepted|Declined|Attended|Absent)$", ErrorMessage = "{0} must be Invited, Accepted, Declined, Attended or Absent.")]
    public string AttendeeStatus { get; set; } = null!;
}
