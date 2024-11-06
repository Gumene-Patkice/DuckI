using System.Diagnostics;
using DuckI.Data;
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

    [HttpGet]
    public IActionResult UploadCalendar()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadCalendar(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var userId = _userManager.GetUserId(User);
            await _calendarService.UploadCalendarAsync(file, userId);
            TempData["UploadSuccess"] = true;
        }
        else
        {
            TempData["UploadSuccess"] = false;
        }
        return RedirectToAction("Index");
    }
}