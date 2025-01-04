using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

public class ManageUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IManageUsersService _manageUsersService;
    
    public ManageUsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IManageUsersService manageUsersService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _manageUsersService = manageUsersService;
    }
    
    [Authorize(Roles="Admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteUserData([FromForm] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        // depending on the role, call the appropriate service method to delete the user data
        if (roles.Contains("Educator"))
        {
            await _manageUsersService.DeleteEducatorData(userId);
        }
        else if (roles.Contains("SuperStudent"))
        {
            await _manageUsersService.DeleteStudentData(userId);
        }
        else if (roles.Contains("Reviewer"))
        {
            await _manageUsersService.DeleteReviewerData(userId);
        }
        else if (roles.Contains("Admin"))
        {
            await _manageUsersService.DeleteAdminData(userId);
        }
        else
        {
            await _manageUsersService.DeleteDefaultUserData(userId);
        }

        // YOU COULD ADD FORCE LOGOUT HERE, DELETE THIS COMMENT LATER LEO
        // (you can also add it somewhere else)
        return Ok("User data deleted successfully.");
    }
}