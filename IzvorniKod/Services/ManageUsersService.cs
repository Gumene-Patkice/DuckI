using DuckI.Data;
using DuckI.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

public interface IManageUsersService
{
    Task DeleteEducatorData(string userId);
    Task DeleteStudentData(string userId);
    Task DeleteReviewerData(string userId);
    Task DeleteAdminData(string userId);
    Task DeleteDefaultUserData(string userId);
    Task<List<UserInfoDto>> GetAllUsersAsync();
}

public class ManageUsersService : IManageUsersService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ManageUsersService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
        _userManager = userManager;
    }

    public async Task DeleteEducatorData(string userId)
    {
        // select all educatorPdfs where userId matches the given userId
        var educatorPdfs = await _context.EducatorPdfs.Where(ep => ep.UserId == userId).ToListAsync();
        // extract all publicPdfIds from the previous line
        var publicPdfIds = educatorPdfs.Select(ep => ep.PublicPdfId).ToList();

        // removing content from our part of the database
        _context.RemovedLogs.RemoveRange(_context.RemovedLogs.Where(rl => rl.EducatorId == userId));
        _context.PublicPdfTags.RemoveRange(_context.PublicPdfTags.Where(ppt => publicPdfIds.Contains(ppt.PublicPdfId)));
        _context.EducatorPdfs.RemoveRange(_context.EducatorPdfs.Where(ep => ep.UserId == userId));
        _context.PublicPdfs.RemoveRange(_context.PublicPdfs.Where(pp => publicPdfIds.Contains(pp.PublicPdfId)));
        _context.FlaggedPdfs.RemoveRange(_context.FlaggedPdfs.Where(fp => publicPdfIds.Contains(fp.PublicPdfId)));
        _context.RatingLogs.RemoveRange(_context.RatingLogs.Where(rl => publicPdfIds.Contains(rl.PublicPdfId)));
        _context.UserCalendars.RemoveRange(_context.UserCalendars.Where(uc => uc.UserId == userId));
        _context.UserRoleStatuses.RemoveRange(_context.UserRoleStatuses.Where(urs => urs.UserId == userId));
        _context.Calendars.RemoveRange(_context.Calendars.Where(c => c.UserCalendar.UserId == userId));

        await _context.SaveChangesAsync();
        // deleting identity user
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);

        // deleting calendars
        var calendarDirectory = Path.Combine("Data", "Files", "Calendars");
        var calendarFiles = Directory.GetFiles(calendarDirectory, $"{userId}.csv");
        foreach (var file in calendarFiles)
        {
            File.Delete(file);
        }

        // deleting public pdfs
        var publicPdfsDirectory = Path.Combine("Data", "Files", "PublicPdfs");
        var publicPdfFiles = Directory.GetFiles(publicPdfsDirectory, $"{userId}$*.pdf");
        foreach (var file in publicPdfFiles)
        {
            File.Delete(file);
        }
    }

    public async Task DeleteStudentData(string userId)
    {
        // select all studentPdfs where userId matches the given userId
        var studentPdfs = await _context.StudentPdfs.Where(sp => sp.UserId == userId).ToListAsync();
        // extract all privatePdfIds from the previous line
        var privatePdfIds = studentPdfs.Select(sp => sp.PrivatePdfId).ToList();

        // removing content from our part of the database
        _context.PrivatePdfTags.RemoveRange(_context.PrivatePdfTags.Where(ppt => privatePdfIds.Contains(ppt.PrivatePdfId)));
        _context.StudentPdfs.RemoveRange(_context.StudentPdfs.Where(sp => sp.UserId == userId));
        _context.PrivatePdfs.RemoveRange(_context.PrivatePdfs.Where(pp => privatePdfIds.Contains(pp.PrivatePdfId)));
        _context.FlaggedPdfs.RemoveRange(_context.FlaggedPdfs.Where(fp => fp.UserId == userId));
        _context.RatingLogs.RemoveRange(_context.RatingLogs.Where(rl => rl.UserId == userId));
        _context.UserCalendars.RemoveRange(_context.UserCalendars.Where(uc => uc.UserId == userId));
        _context.UserRoleStatuses.RemoveRange(_context.UserRoleStatuses.Where(urs => urs.UserId == userId));
        _context.Calendars.RemoveRange(_context.Calendars.Where(c => c.UserCalendar.UserId == userId));

        await _context.SaveChangesAsync();
        // deleting identity user
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);

        // deleting calendars
        var calendarDirectory = Path.Combine("Data", "Files", "Calendars");
        var calendarFiles = Directory.GetFiles(calendarDirectory, $"{userId}.csv");
        foreach (var file in calendarFiles)
        {
            File.Delete(file);
        }

        // deleting private pdfs
        var privatePdfsDirectory = Path.Combine("Data", "Files", "PrivatePdfs");
        var privatePdfFiles = Directory.GetFiles(privatePdfsDirectory, $"{userId}$*.pdf");
        foreach (var file in privatePdfFiles)
        {
            File.Delete(file);
        }
    }

    public async Task DeleteReviewerData(string userId)
    {
        // removing content from our part of the database
        _context.RemovedLogs.RemoveRange(_context.RemovedLogs.Where(rl => rl.ReviewerId == userId));
        _context.UserCalendars.RemoveRange(_context.UserCalendars.Where(uc => uc.UserId == userId));
        _context.UserRoleStatuses.RemoveRange(_context.UserRoleStatuses.Where(urs => urs.UserId == userId));
        _context.Calendars.RemoveRange(_context.Calendars.Where(c => c.UserCalendar.UserId == userId));

        await _context.SaveChangesAsync();
        // deleting identity user
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);

        // deleting calendars
        var calendarDirectory = Path.Combine("Data", "Files", "Calendars");
        var calendarFiles = Directory.GetFiles(calendarDirectory, $"{userId}.csv");
        foreach (var file in calendarFiles)
        {
            File.Delete(file);
        }
    }

    public async Task DeleteAdminData(string userId)
    {
        // removing content from our part of the database
        _context.UserCalendars.RemoveRange(_context.UserCalendars.Where(uc => uc.UserId == userId));
        _context.Calendars.RemoveRange(_context.Calendars.Where(c => c.UserCalendar.UserId == userId));

        await _context.SaveChangesAsync();
        // deleting identity user
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);

        // deleting calendars
        var calendarDirectory = Path.Combine("Data", "Files", "Calendars");
        var calendarFiles = Directory.GetFiles(calendarDirectory, $"{userId}.csv");
        foreach (var file in calendarFiles)
        {
            File.Delete(file);
        }
    }

    public async Task DeleteDefaultUserData(string userId)
    {
        // delete identity user
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);
    }
    
    /// <summary>
    /// this is used for the controller to get all users
    /// </summary>
    public async Task<List<UserInfoDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userInfoList = new List<UserInfoDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var role = roles.FirstOrDefault();
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                role = "Admin";
            }
            else if (await _userManager.IsInRoleAsync(user, "SuperStudent"))
            {
                role = "Student";
            }
            else if (await _userManager.IsInRoleAsync(user, "Educator"))
            {
                role = "Educator";
            }
            else if (await _userManager.IsInRoleAsync(user, "Reviewer"))
            {
                role = "Reviewer";
            }
            else
            {
                role = "Default User";
            }

            userInfoList.Add(new UserInfoDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Role = role
            });
        }

        return userInfoList;
    }
}