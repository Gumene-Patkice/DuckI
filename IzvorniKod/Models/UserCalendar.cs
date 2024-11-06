using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class UserCalendar
{
    public string UserId { get; set; }
    public int CalendarId { get; set; }
    public IdentityUser User { get; set; }
    public Calendar Calendar { get; set; }
}