using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

[Route("api/calendars")]
[ApiController]
public class CalendarApiController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly UserManager<IdentityUser> _userManager;

    public CalendarApiController(ICalendarService calendarService, UserManager<IdentityUser> userManager)
    {
        _calendarService = calendarService;
        _userManager = userManager;
    }
    
    [Authorize]
    [HttpGet("getcalendar")]
    public async Task<IActionResult> GetCalendar()
    {
        var userId = _userManager.GetUserId(User);
        var fileBytes = await _calendarService.GetCalendarAsync(userId);

        if (fileBytes == null)
        {
            return NotFound("Calendar not found.");
        }

        return File(fileBytes, "text/csv", $"{userId}.csv");
    }
}