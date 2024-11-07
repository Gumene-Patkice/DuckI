using DuckI.Data;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

/// <summary>
/// This class provides services for managing calendars.
/// </summary>
/// <remarks>
/// This service includes methods for uploading and retrieving calendar files.
/// </remarks>
public interface ICalendarService
{
    Task UploadCalendarAsync(IFormFile file, string userId); // this method doesn't belong to api/calendars, but it is used in HomeController
    Task<byte[]> GetCalendarAsync(string userId);
}

/// <summary>
/// This class implements services for managing calendars.
/// </summary>
public class CalendarService : ICalendarService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;

    public CalendarService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    /// <summary>
    /// Uploads a calendar file to the server.
    /// </summary>
    /// <param name="file">.csv file</param>
    /// <param name="userId">id of the user uploading calendar</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task UploadCalendarAsync(IFormFile file, string userId)
    {
        // first check if the file is null or empty
        if (file != null && file.Length > 0)
        {
            // then check if the file ends with .csv
            // if it doesn't throw an error
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".csv")
            {
                throw new InvalidOperationException("Invalid file type. Only .csv files are allowed.");
            }
            
            // get a record from UserCalendars which will be used to check if the user with userId already has a calendar
            var existingUserCalendar = _context.UserCalendars.FirstOrDefault(uc => uc.UserId == userId);
            
            // check if the user has a calendar
            if (existingUserCalendar != null)
            {
                // if the user has a calendar, delete the calendar and the userCalendar record
                // in both UserCalendars and Calendars tables
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
            // using id to name calendar files to avoid problems if two users have the same file name
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Calendars", userId + ".csv");

            // save the file to the directory
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // add calendar to the table
            var calendar = new Calendar { CsvPath = filePath };
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();
    
            // add record to UserCalendar table
            var userCalendar = new UserCalendar
            {
                UserId = userId,
                CalendarId = calendar.CalendarId
            };
            _context.UserCalendars.Add(userCalendar);
            
            // save changes
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Uploads a calendar file to the server.
    /// </summary>
    /// <param name="userId">id of the user who is requesting the file</param>
    /// <returns> byte array of the calendar file if it exists, otherwise null</returns>
    public async Task<byte[]> GetCalendarAsync(string userId)
    {
        // get the userCalendar record for the user, combined with the calendar record
        var userCalendar = await _context.UserCalendars
            .Include(uc => uc.Calendar)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCalendar == null || userCalendar.Calendar == null)
        {
            return null;
        }
        
        // get the file path and check if it exists
        var filePath = userCalendar.Calendar.CsvPath;
        if (!System.IO.File.Exists(filePath))
        {
            return null;
        }

        // return the file as a byte array
        return await System.IO.File.ReadAllBytesAsync(filePath);
    }
}