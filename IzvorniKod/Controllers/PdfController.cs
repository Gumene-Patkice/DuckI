using DuckI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DuckI.Controllers;

public class PdfController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUploadPdfService _uploadPdfService;
    private readonly ITagService _tagService;
    
    public PdfController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IUploadPdfService uploadPdfService, ITagService tagService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _uploadPdfService = uploadPdfService;
        _tagService = tagService;
    }
    
    [Authorize]
    public async Task<IActionResult> UploadPdf()
    {
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Tags = new SelectList(tags, "TagName", "TagName");
        return View();
    }
    
    //[Authorize(Roles="SuperStudent")]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadPrivatePdf(IFormFile file, string tagName)
    {
        if (file == null || string.IsNullOrEmpty(tagName))
        {
            return BadRequest("File and tag name are required.");
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await _uploadPdfService.UploadPrivatePdfAsync(file, userId, tagName);
            return Ok("PDF uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [Authorize(Roles="Educator")]
    [HttpPost]
    public async Task<IActionResult> UploadPublicPdf(IFormFile file, string tagName)
    {
        if (file == null || string.IsNullOrEmpty(tagName))
        {
            return BadRequest("File and tag name are required.");
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await _uploadPdfService.UploadPublicPdfAsync(file, userId, tagName);
            return Ok("PDF uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    
}