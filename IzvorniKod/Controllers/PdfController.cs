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
    private readonly IManagePdfService _managePdfService;
    
    public PdfController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IUploadPdfService uploadPdfService, ITagService tagService, IManagePdfService managePdfService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _uploadPdfService = uploadPdfService;
        _tagService = tagService;
        _managePdfService = managePdfService;
    }
    
    [Authorize(Roles = "SuperStudent,Educator")]
    public async Task<IActionResult> UploadPdf()
    {
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Tags = new SelectList(tags, "TagName", "TagName");
        return View();
    }
    
    [Authorize(Roles="SuperStudent")]
    //[Authorize]
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
    
    [Authorize]
    public async Task<IActionResult> ViewPublicMaterial()
    {
        var pdfs = await _managePdfService.GetAllPublicPdfsAsync();
        return View(pdfs);
    }
    
    [Authorize(Roles="SuperStudent")]
    // [Authorize(Roles="SuperStudent,Educator")]
    [HttpPost]
    public async Task<IActionResult> FlagPdf([FromForm] long publicPdfId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await _managePdfService.AddUserToFlaggedPdfsAsync(userId, publicPdfId);
            return Ok("PDF flagged successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [Authorize(Roles="SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> OpenPrivatePdf([FromForm] long pdfId)
    {
        var pdfPath = await _managePdfService.GetPdfPathByIdAsync(pdfId, "false");
        if (pdfPath == null)
        {
            return NotFound("PDF not found.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        return File(fileBytes, "application/pdf");
    }

    [Authorize(Roles="SuperStudent,Educator")]
    [HttpPost]
    public async Task<IActionResult> OpenPublicPdf([FromForm] long pdfId)
    {
        var pdfPath = await _managePdfService.GetPdfPathByIdAsync(pdfId, "true");
        if (pdfPath == null)
        {
            return NotFound("PDF not found.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        return File(fileBytes, "application/pdf");
    }
    
    [Authorize(Roles="SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> UnflagPdf([FromForm] long publicPdfId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await _managePdfService.RemoveUserFromFlaggedPdfsAsync(userId, publicPdfId);
            return Ok("PDF unflagged successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}