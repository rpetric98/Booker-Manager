using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public string? UserRoleName { get; set; }

    public virtual ICollection<UserDetail> UserDetails { get; } = new List<UserDetail>();
}
