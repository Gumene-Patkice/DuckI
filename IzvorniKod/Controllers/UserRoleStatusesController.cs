using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

/// <summary>
/// This class provides api routes implementation for UserRoleStatuses get requests.
/// </summary>
/// <remarks>
/// This class includes api/userrolestatuses routes for UserRoleStatuses get requests.
/// </remarks>
[Route("api/userrolestatuses")]
[ApiController]
public class UserRoleStatusesController : ControllerBase
{
    private readonly IUserRoleStatusesService _userRoleStatusesService;
    private readonly UserManager<IdentityUser> _userManager;

    public UserRoleStatusesController(IUserRoleStatusesService userRoleStatusesService, UserManager<IdentityUser> userManager)
    {
        _userRoleStatusesService = userRoleStatusesService;
        _userManager = userManager;
    }
    
    /// <summary>Get all roles from the UserRoleStatuses table route.</summary>
    //[Authorize(Roles = "Admin")] // WILL BE ADDED LATER, FOR DEVOLPMENT PURPOSES WE LEAVE IT AS IT IS
    [Authorize] // CHANGE LATER WITH ROLE ADMIN
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllUserRoleStatuses()
    {
        var userRoleStatuses = await _userRoleStatusesService.GetAllUserRoleStatusesAsync();
        
        if(userRoleStatuses == null || !userRoleStatuses.Any())
        {
            return NotFound();
        }
        
        return Ok(userRoleStatuses);
    }
}