namespace SmartMeetingRoom.API.Models;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public string CreatedByIp { get; set; } = null!;

    public int ApplicationUserId { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
