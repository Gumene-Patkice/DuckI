using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

/// <summary>
/// This class provides api routes implementation for calendar get requests.
/// </summary>
/// <remarks>
/// This class includes api/calendars routes for calendar get requests.
/// </remarks>
[Route("api/calendars")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly UserManager<IdentityUser> _userManager;

    public CalendarController(ICalendarService calendarService, UserManager<IdentityUser> userManager)
    {
        _calendarService = calendarService;
        _userManager = userManager;
    }
    
    ///<summary>Get calendar file route</summary>
    [Authorize(Roles = "Admin,SuperStudent,Educator,Reviewer")]
    [HttpGet("getcalendar")]
    public async Task<IActionResult> GetCalendar()
    {
        // get the current user's id and calendar for the current user
        var userId = _userManager.GetUserId(User);
        var fileBytes = await _calendarService.GetCalendarAsync(userId);

        if (fileBytes == null)
        {
            var defaultCsvContent = "1990-01-01,ThisEventHereWasAddedByDuckITeamSoThatYourCalendarCouldWork";
            var defaultCalendarBytes = System.Text.Encoding.UTF8.GetBytes(defaultCsvContent);

            using (var stream = new MemoryStream(defaultCalendarBytes))
            {
                var formFile = new FormFile(stream, 0, defaultCalendarBytes.Length, "default_calendar", "default_calendar.csv")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/csv"
                };
                await _calendarService.UploadCalendarAsync(formFile, userId);
            }

            fileBytes = await _calendarService.GetCalendarAsync(userId);
        }

        if (fileBytes == null)
        {
            return NotFound("Calendar not found.");
        }

        // return the calendar file, if it is found
        return File(fileBytes, "text/csv", $"{userId}.csv");
    }
    
    [Authorize(Roles = "Admin,SuperStudent,Educator,Reviewer")]
    [HttpPost("addevent")]
    public async Task<IActionResult> AddEvent([FromQuery] string eventDate, [FromQuery] string eventDescription)
    {
        var userId = _userManager.GetUserId(User);

        if (!DateTime.TryParse(eventDate, out var parsedEventDate))
        {
            return BadRequest("Invalid date format.");
        }

        await _calendarService.AddEventToCalendarAsync(parsedEventDate, eventDescription, userId);
        return Ok("Event added successfully.");
    }
    
    [Authorize(Roles = "Admin,SuperStudent,Educator,Reviewer")]
    [HttpDelete("deleteevent")]
    public async Task<IActionResult> DeleteEvent([FromQuery] string eventDate, [FromQuery] string eventDescription)
    {
        var userId = _userManager.GetUserId(User);

        if (!DateTime.TryParse(eventDate, out var parsedEventDate))
        {
            return BadRequest("Invalid date format.");
        }

        try
        {
            await _calendarService.DeleteEventFromCalendarAsync(parsedEventDate, eventDescription, userId);
            return Ok("Event deleted successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}