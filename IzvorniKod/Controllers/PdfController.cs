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
    
    /// <summary>
    /// Renders UploadPdf view, as well as get all tags.
    /// </summary>
    [Authorize(Roles = "SuperStudent,Educator")]
    public async Task<IActionResult> UploadPdf()
    {
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Tags = new SelectList(tags, "TagName", "TagName");
        return View();
    }
    
    /// <summary>
    /// Used in UploadPdf view.
    /// Accessible only to students (SuperStudents).
    /// </summary>
    [Authorize(Roles="SuperStudent")]
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
    
    /// <summary>
    /// Used in UploadPdf.
    /// Accessible only to Educators.
    /// </summary>
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
    
    /// <summary>
    /// Used to render the ViewPublicMaterial view with all available public PDFs
    /// </summary>
    [Authorize]
    public async Task<IActionResult> ViewPublicMaterial()
    {
        var pdfs = await _managePdfService.GetAllPublicPdfsAsync();
        return View(pdfs);
    }
    
    /// <summary>
    /// Used in javascript in ViewPublicMaterial view for flagging public PDFs.
    /// </summary>
    [Authorize(Roles="SuperStudent")]
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
    
    /// <summary>
    /// Used in Index view to open private PDFs.
    /// </summary>
    [Authorize(Roles="SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> OpenPrivatePdf([FromForm] long pdfId)
    {
        var pdfPath = await _managePdfService.GetPdfPathByIdAsync(pdfId, false);
        if (pdfPath == null)
        {
            return NotFound("PDF not found.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        return File(fileBytes, "application/pdf");
    }

    /// <summary>
    ///  Used in both Index and ViewPublicMaterial views to open public PDFs.
    /// </summary>
    [Authorize(Roles="SuperStudent,Educator,Reviewer")]
    [HttpPost]
    public async Task<IActionResult> OpenPublicPdf([FromForm] long pdfId)
    {
        var pdfPath = await _managePdfService.GetPdfPathByIdAsync(pdfId, true);
        if (pdfPath == null)
        {
            return NotFound("PDF not found.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        return File(fileBytes, "application/pdf");
    }
    
    /// <summary>
    /// Used in javascript in Index view for unflagging previously flagged public PDFs.
    /// </summary>
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
    
    ///<summary>
    /// Rate public PDFs.
    /// Accessible only to students (SuperStudents)
    /// Used in ViewPublicMaterial view.
    /// </summary>
    [Authorize(Roles="SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> RatePdf([FromForm] long pdfId, [FromForm] string isUpvote)
    {
        var userId = _userManager.GetUserId(User);
        // forms can't pass boolean values, so we pass them as strings 
        await _managePdfService.RatePdfAsync(pdfId, userId, isUpvote == "true");
        return RedirectToAction("ViewPublicMaterial");
    }
    
    /// <summary>
    /// Used in javascript Index view and ViewPublicMaterial view to get all tags.
    /// </summary>
    [Authorize(Roles="Educator,SuperStudent,Reviewer,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Json(tags);
    }
    
    /// <summary>
    /// Used in Index view to delete private PDFs by students (SuperStudents).
    /// </summary>
    [Authorize(Roles="SuperStudent")]
    [HttpPost]
    public async Task<IActionResult> DeletePrivatePdf([FromForm] long pdfId)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            await _managePdfService.DeletePrivatePdfAsync(pdfId, userId);
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Used by Educators in Index view to delete their public PDFs.
    /// </summary>
    [Authorize(Roles="Educator")]
    [HttpPost]
    public async Task<IActionResult> DeletePublicPdf([FromForm] long pdfId)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            await _managePdfService.DeletePublicPdfAsync(pdfId, userId);
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Used by Reviewers in ViewPublicMaterial view to delete public PDFs.
    /// </summary>
    [Authorize(Roles = "Reviewer")]
    [HttpPost]
    public async Task<IActionResult> DeletePublicPdfReviewer([FromForm] long pdfId, [FromForm] string description)
    {
        try
        {
            var reviewerId = _userManager.GetUserId(User);
            await _managePdfService.DeletePublicPdfReviewerAsync(pdfId, reviewerId, description);
            return RedirectToAction("ViewPublicMaterial");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [Authorize(Roles = "Educator")]
    public async Task<IActionResult> ViewRemovedLogs()
    {
        var educatorId = _userManager.GetUserId(User);
        var removedLogs = await _managePdfService.GetAllRemovedLogsAsync(educatorId);
        return View(removedLogs);
    }
    
    [Authorize(Roles="SuperStudent,Educator")]
    [HttpGet]
    public async Task<IActionResult> FetchPdfByName([FromQuery] string pdfName, [FromQuery] bool isPublic)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(pdfName))
        {
            return BadRequest("PDF name is required.");
        }

        try
        {
            var exists = await _managePdfService.FetchPdfByNameAsync(pdfName, userId, isPublic);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
}