using System.Web;
using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

[Route("api/chat")]
[ApiController]
public class ChatApiController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly UserManager<IdentityUser> _userManager;

    public ChatApiController(IChatService chatService, UserManager<IdentityUser> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
    }
    
    [Authorize]
    [HttpPost("prompt")]
    public async Task<IActionResult> ExecutePrompt([FromQuery] string p)
    {
        var prompt = HttpUtility.UrlDecode(p);
        if (string.IsNullOrEmpty(prompt))
        {
            return BadRequest("Prompt cannot be empty.");
        }

        var response = await _chatService.PromptAsync(prompt);
        if (response == null)
        {
            return StatusCode(500, "Error generating response.");
        }

        return Ok(response);
    }
}