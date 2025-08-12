using Microsoft.AspNetCore.Identity;

namespace SmartMeetingRoom.API.Models;

public class ApplicationRole : IdentityRole<int>
{
    public string? RoleDescription { get; set; }

    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}