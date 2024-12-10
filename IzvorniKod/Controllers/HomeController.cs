using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DuckI.Models;
using DuckI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace DuckI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICalendarService _calendarService;
    private readonly IUserRoleStatusesService _userRoleStatusesService;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ICalendarService calendarService,
        UserManager<IdentityUser> userManager, IUserRoleStatusesService userRoleStatusesService)
    {
        _logger = logger;
        _calendarService = calendarService;
        _userRoleStatusesService = userRoleStatusesService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [Authorize]
    public IActionResult Chat()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    ///<summary>
    /// Check current user's role status and display the view with TempData (will be used for status)
    ///</summary>
    [Authorize]
    public async Task<IActionResult> Roles()
    {
        var userId = _userManager.GetUserId(User);
        var userRecord = await _userRoleStatusesService.GetUserRoleStatusByUserIdAsync(userId);
        
        if (userRecord != null)
        {
            TempData["UserRoleStatus"] = userRecord.Status ? "Approved" : "Pending";
        }
        else
        {
            TempData["UserRoleStatus"] = "Unknown";
        }
        
        return View();
    }

    ///<summary>Render UploadCalendar view</summary>
    [Authorize]
    [HttpGet]
    public IActionResult UploadCalendar()
    {
        return View();
    }

    ///<summary>Upload a calendar file route</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadCalendar(IFormFile file)
    {
        // check if the file is uploaded
        if (file != null && file.Length > 0)
        {
            // get userId and upload calendar for the user
            var userId = _userManager.GetUserId(User);
            await _calendarService.UploadCalendarAsync(file, userId);
            TempData["UploadSuccess"] = true;
        }
        else
        {
            TempData["UploadSuccess"] = false;
        }
        
        // redirect to the Index page
        return RedirectToAction("Index");
    }
    
    ///<summary>Apply for role. Adds records to the UserRoleStatuses table.</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUserRoleStatus([FromForm] string roleName, [FromForm] string description)
    {
        var userId = _userManager.GetUserId(User);
        
        // Check if the user is in the Admin role (Admins can't apply for roles)
        var isAdmin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Admin");
        if (isAdmin)
        {
            TempData["AppliedForRole"] = false;
            // TempData["ErrorMessage"] = "Admins cannot apply for roles."; // maybe for later implementation
            return RedirectToAction("Roles");
        }
        
        try
        {
            await _userRoleStatusesService.AddUserRoleStatusAsync(userId, roleName, description);
            TempData["AppliedForRole"] = true;
            return RedirectToAction("Roles");
        }
        catch (Exception e)
        {
            TempData["AppliedForRole"] = false;
            return RedirectToAction("Roles");
        }
    }
    
    ///<summary> Apply for role. Adds records to the UserRoleStatuses table. </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUserToSuperStudentRole([FromForm] string roleName)
    {
        var userId = _userManager.GetUserId(User);
        
        // Check if the user is in the Admin role (Admins can't apply for roles)
        var isAdmin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Admin");
        if (isAdmin)
        {
            TempData["AppliedForRole"] = false;
            return RedirectToAction("Roles");
        }
        
        try
        {
            await _userRoleStatusesService.AssignSuperStudentAsync(userId, roleName);
            TempData["AppliedForRole"] = true;
            return RedirectToAction("Roles");
        }
        catch (Exception ex)
        {
            TempData["AppliedForRole"] = false;
            return RedirectToAction("Roles");
        }
    }
}