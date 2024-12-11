using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class UserRoleStatus
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
    public bool Status { get; set; } // true = approved, false = pending
    public string Description { get; set; }
    public IdentityUser User { get; set; }
    public IdentityRole Role { get; set; }
}