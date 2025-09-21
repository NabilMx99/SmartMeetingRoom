using System.Text.Json;

namespace SmartMeetingRoom.API.Services;

public interface IZoomService
{
    Task<string> GetAccessTokenAsync();
    Task<JsonElement> CreateMeetingAsync(string meetingTitle, string? meetingAgenda, DateTime meetingStartTime, DateTime meetingEndTime);
    Task<JsonElement> GetMeetingAsync(long zoomMeetingId);
    Task<bool> DeleteMeetingAsync(long zoomMeetingId);
}
