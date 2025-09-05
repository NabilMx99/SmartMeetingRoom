using System.ComponentModel.DataAnnotations;

namespace SmartMeetingRoom.API.DTOs.Meeting;

public class MeetingCreateDto : IValidatableObject
{
    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Room Id")]
    public int FkRoomId { get; set; }

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Meeting Start Time")]
    public DateTime MeetingStartTime { get; set; }

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Meeting End Time")]
    public DateTime MeetingEndTime { get; set; }

    [Required(ErrorMessage = "{0} is required.")]
    [Display(Name = "Meeting Title")]
    [MaxLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
    public string MeetingTitle { get; set; } = null!;

    [Display(Name = "Meeting Agenda")]
    public string? MeetingAgenda { get; set; }

    [Display(Name = "Attendees")]
    [MinLength(1, ErrorMessage = "At least {1} attendee must be selected.")]
    public ICollection<int> AttendeeUserIds { get; set; } = new List<int>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MeetingEndTime <= MeetingStartTime)
        {
            yield return new ValidationResult(
                "Meeting end time must be later than start time.",
                new[] { nameof(MeetingEndTime) });
        }

        if (MeetingStartTime < DateTime.Now)
        {
            yield return new ValidationResult(
                "Meeting start time cannot be in the past.",
                new[] { nameof(MeetingStartTime) });
        }
    }
}

