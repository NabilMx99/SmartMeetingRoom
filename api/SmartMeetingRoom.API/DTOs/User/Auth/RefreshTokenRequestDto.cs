namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class RefreshTokenRequestDto
{
    public string Token { get; set; } = null!;

    public string IpAddress { get; set; } = null!;
}
