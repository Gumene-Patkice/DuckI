using System.Diagnostics;
using DuckI.Dtos;
using DuckI.Models;
using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuckI.Controllers;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserRoleStatusesService _userRoleStatusesService;
    private readonly ITagService _tagService;
    
    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IUserRoleStatusesService userRoleStatusesService, ITagService tagService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRoleStatusesService = userRoleStatusesService;
        _tagService = tagService;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult ControlPanel()
    {
        return View();
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BrowseRoleApplications()
    {
        var userRoleStatusDtos = await _userRoleStatusesService.GetAllUserRoleStatusesAsync();
        var browseRoleApplicationsDtos = new List<BrowseRoleApplicationsDto>();

        foreach (var dto in userRoleStatusDtos)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            var role = await _roleManager.FindByIdAsync(dto.RoleId);
            

            browseRoleApplicationsDtos.Add(new BrowseRoleApplicationsDto
            {
                Email = user.Email,
                UserId = dto.UserId,
                RoleName = role.Name,
                RoleId = dto.RoleId,
                Status = dto.Status,
                Description = dto.Description
            });
        }

        return View(browseRoleApplicationsDtos);
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult Tags()
    {
        return View();
    }
    
    [Authorize(Roles = "Admin")]
    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole([FromForm] string userId, [FromForm] string roleId)
    {
        try
        {
            await _userRoleStatusesService.AddUserToRoleAsync(userId, roleId);
            return RedirectToAction("BrowseRoleApplications");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "InternalServerError", message = ex.Message });
        }
    }
    
    [Authorize(Roles = "Admin")]
    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> RejectUser([FromForm] string userId, [FromForm] string roleId)
    {
        try
        {
            await _userRoleStatusesService.RejectUserAsync(userId, roleId);
            return RedirectToAction("BrowseRoleApplications");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "InternalServerError", message = ex.Message });
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddTag([FromForm] string tagName) {
        try
        {
            await _tagService.AddTagAsync(tagName);
            TempData["TagAdded"] = true;
            return RedirectToAction("Tags");
        }
        catch (Exception ex)
        {
            TempData["TagAdded"] = false;
            return RedirectToAction("Tags");
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Tags = tags;
        return View("Tags");
    }
}