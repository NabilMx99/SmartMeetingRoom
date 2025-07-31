using System;
using System.Collections.Generic;

namespace SmartMeetingRoom.API.Models;

public partial class Role
{
    public byte RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
