using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartMeetingRoom.API.Services;

public class ZoomService : IZoomService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _accessToken;
    private DateTime _tokenExpiry;

    public ZoomService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
            return _accessToken;

        var accountId = _configuration["Zoom:AccountId"];
        var clientId = _configuration["Zoom:ClientId"];
        var clientSecret = _configuration["Zoom:ClientSecret"];

        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}"
        );
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (!result.TryGetProperty("access_token", out var tokenProp) || tokenProp.GetString() == null)
            throw new Exception("Zoom token response invalid.");

        _accessToken = tokenProp.GetString();
        var expiresIn = result.GetProperty("expires_in").GetInt32();
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

        return _accessToken!;
    }

    public async Task<JsonElement> CreateMeetingAsync(string meetingTitle, string? meetingAgenda, DateTime meetingStartTime, DateTime meetingEndTime)
    {
        var token = await GetAccessTokenAsync();

        var payload = new
        {
            topic = meetingTitle,
            agenda = meetingAgenda,
            start_time = meetingStartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            duration = (int)(meetingEndTime - meetingStartTime).TotalMinutes,
            settings = new
            {
               allow_host_control_participant_mute_state = true,
               allow_multiple_devices = true,
               auto_start_meeting_summary = true,
               encryption_type = "enhanced_encryption",
               host_video = false,
               mute_upon_entry = true,
               private_meeting = true 
            },
            type = 2
        };

        using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zoom.us/v2/users/me/meetings")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json;
    }

    public async Task<JsonElement> GetMeetingAsync(long zoomMeetingId)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.zoom.us/v2/meetings/{zoomMeetingId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json;
    }

    public async Task<bool> DeleteMeetingAsync(long zoomMeetingId)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"https://api.zoom.us/v2/meetings/{zoomMeetingId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}

