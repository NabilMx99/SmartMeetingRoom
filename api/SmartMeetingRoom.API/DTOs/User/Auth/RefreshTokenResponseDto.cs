namespace SmartMeetingRoom.API.DTOs.User.Auth;

public class RefreshTokenResponseDto
{
    public string Token { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;
}
