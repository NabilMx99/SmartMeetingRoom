using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Meeting;

public class MeetingUpdateDto : IValidatableObject
{
    [Display(Name = "Meeting Start Time")]
    public DateTime? MeetingStartTime { get; set; }

    [Display(Name = "Meeting End Time")]
    public DateTime? MeetingEndTime { get; set; }

    [Display(Name = "Meeting Title")]
    [MaxLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string? MeetingTitle { get; set; }

    [Display(Name = "Meeting Agenda")]
    public string? MeetingAgenda { get; set; }

    [Display(Name = "Meeting Status")]
    [MaxLength(20, ErrorMessage = "{0} cannot exceed {1} characters.")]
    [RegularExpression("^(Scheduled|Ongoing|Completed|Cancelled)$", ErrorMessage = "{0} must be one of the following: Scheduled, Ongoing, Completed, or Cancelled.")]
    public string? MeetingStatus { get; set; }

    [Display(Name = "Attendees")]
    public ICollection<int>? AttendeeUserIds { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MeetingStartTime.HasValue && MeetingEndTime.HasValue)
        {
            if (MeetingEndTime <= MeetingStartTime)
            {
                yield return new ValidationResult(
                    "Meeting end time must be later than start time.",
                    new[] { nameof(MeetingEndTime) });
            }
        }

        if (MeetingStartTime.HasValue && MeetingStartTime.Value < DateTime.Now)
        {
            yield return new ValidationResult(
                "Meeting start time cannot be in the past.",
                new[] { nameof(MeetingStartTime) });
        }
    }
}