using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserDetail
{
    public int UserDetailId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? UserRoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserRole? UserRole { get; set; }
}
