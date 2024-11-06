using DuckI.Data;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

public interface ICalendarService
{
    Task UploadCalendarAsync(IFormFile file, string userId);
    Task<byte[]> GetCalendarAsync(string userId);
}

public class CalendarService : ICalendarService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;

    public CalendarService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    public async Task UploadCalendarAsync(IFormFile file, string userId)
    {
        if (file != null && file.Length > 0)
        {
            var existingUserCalendar = _context.UserCalendars.FirstOrDefault(uc => uc.UserId == userId);

            if (existingUserCalendar != null)
            {
                var existingCalendar = await _context.Calendars.FindAsync(existingUserCalendar.CalendarId);
                if (existingCalendar != null)
                {
                    var existingFilePath = existingCalendar.CsvPath;
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }

                    _context.Calendars.Remove(existingCalendar);
                    _context.UserCalendars.Remove(existingUserCalendar);
                    await _context.SaveChangesAsync();
                }
            }
            
            // Directory.GetCurrentDirectory()
            // using id to name calendar files to avoid probles if two users have the same file name
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Calendars", userId + ".csv");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var calendar = new Calendar { CsvPath = filePath };
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();

            var userCalendar = new UserCalendar
            {
                UserId = userId,
                CalendarId = calendar.CalendarId
            };
            _context.UserCalendars.Add(userCalendar);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<byte[]> GetCalendarAsync(string userId)
    {
        var userCalendar = await _context.UserCalendars
            .Include(uc => uc.Calendar)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCalendar == null || userCalendar.Calendar == null)
        {
            return null;
        }

        var filePath = userCalendar.Calendar.CsvPath;
        if (!System.IO.File.Exists(filePath))
        {
            return null;
        }

        return await System.IO.File.ReadAllBytesAsync(filePath);
    }
}