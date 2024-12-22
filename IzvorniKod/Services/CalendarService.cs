using DuckI.Data;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

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
    Task AddEventToCalendarAsync(DateTime eventDate, string eventDescription, string userId);
    Task DeleteEventFromCalendarAsync(DateTime eventDate, string eventDescription, string userId);
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
            
            // read the file and check if the .csv is in the correct format
            var dateEvents = new Dictionary<DateTime, int>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length != 2)
                    {
                        throw new InvalidOperationException("Invalid CSV format.");
                    }

                    // out var is used to store eventDate for later use inside this while block
                    // if the date is in the correct format
                    if (!DateTime.TryParse(values[0], out var eventDate))
                    {
                        throw new InvalidOperationException("Invalid date format in CSV.");
                    }
                }
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
            var filePath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Calendars")))
                {
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Calendars"));
                }
                filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Calendars", userId + ".csv");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Calendars")))
                {
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Calendars"));
                }
                filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Calendars", userId + ".csv");
            }

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

    public async Task AddEventToCalendarAsync(DateTime eventDate, string eventDescription, string userId)
    {
        var userCalendar = await _context.UserCalendars
            .Include(uc => uc.Calendar)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCalendar == null || userCalendar.Calendar == null)
        {
            throw new InvalidOperationException("User calendar not found.");
        }

        var filePath = userCalendar.Calendar.CsvPath;
        if (!System.IO.File.Exists(filePath))
        {
            throw new InvalidOperationException("Calendar file not found.");
        }

        var lines = new List<string>();

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lines.Add(line);
            }
        }

        lines.Add($"{eventDate.ToString("yyyy-MM-dd")},{eventDescription}");

        using (var writer = new StreamWriter(filePath))
        {
            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }
        }
    }

    public async Task DeleteEventFromCalendarAsync(DateTime eventDate, string eventDescription, string userId)
    {
        var userCalendar = await _context.UserCalendars
            .Include(uc => uc.Calendar)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCalendar == null || userCalendar.Calendar == null)
        {
            throw new InvalidOperationException("Calendar not found.");
        }

        var filePath = userCalendar.Calendar.CsvPath;
        if (!System.IO.File.Exists(filePath))
        {
            throw new FileNotFoundException("Calendar file not found.");
        }

        var lines = new List<string>();
        var eventLine = $"{eventDate:yyyy-MM-dd},{eventDescription}";

        var counter = 0;
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.Equals(eventLine, StringComparison.OrdinalIgnoreCase))
                {
                    lines.Add(line);
                }
                else
                {
                    counter++;
                }
            }
        }

        if (counter == 0)
        {
            throw new InvalidOperationException("Event not found.");
        }

        using (var writer = new StreamWriter(filePath))
        {
            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }
        }
    }
}