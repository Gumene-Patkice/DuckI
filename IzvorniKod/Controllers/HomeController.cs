using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DuckI.Models;
using DuckI.Services;
using Microsoft.AspNetCore.Identity;

namespace DuckI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICalendarService _calendarService;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ICalendarService calendarService,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _calendarService = calendarService;
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    ///<summary>Render UploadCalendar view</summary>
    [HttpGet]
    public IActionResult UploadCalendar()
    {
        return View();
    }

    ///<summary>Upload a calendar file route</summary>
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
}