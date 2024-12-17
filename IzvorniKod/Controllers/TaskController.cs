using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

public class TaskController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITaskService _taskService;

    public TaskController(UserManager<IdentityUser> userManager, ITaskService taskService)
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
            var pdf = await _taskService.CreateTask(pdfId, isPublic == "true");
            // pdf var is a dto which contains information about pdfId, pdfPath and pdfName
            return Ok("Task created successfully!");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}