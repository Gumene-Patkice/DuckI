using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

public class TasksController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITaskService _taskService;

    public TasksController(UserManager<IdentityUser> userManager, ITaskService taskService)
    {
        _userManager = userManager;
        _taskService = taskService;
    }

    [Authorize(Roles = "SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromForm] long pdfId, [FromForm] string isPublic)
    {
        // bools cannot be passed via forms, so strings (for isPublic) are used instead
        // isPublic is needed to determine from which table the pdf is fetched
        try
        {
            var response = await _taskService.CreateTask(pdfId, isPublic == "true");
            ViewBag.Message = "Task created successfully!";
            ViewBag.Response = response;
            return View("../Home/Tasks");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}