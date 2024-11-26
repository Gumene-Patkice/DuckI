using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuckI.Views.Home;

public class ChatModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<ChatModel> _logger;

    public ChatModel(SignInManager<IdentityUser> signInManager, ILogger<ChatModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }
    
    
}

