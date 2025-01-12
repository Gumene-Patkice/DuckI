using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using DuckI.Data;
using DuckI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


namespace DuckI.Controllers;


public class FlashcardModel
{
    // Define the properties of the flashcard model for easy mapping
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; }

    [JsonPropertyName("answer")]
    public string Answer { get; set; }
}

[Route("api/flashcards")]
[ApiController]
public class FlashcardController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly IWebHostEnvironment
        _webHostEnvironment; // for accessing directory path and other webhostenv functionalities

    private readonly ApplicationDbContext _context;

    public FlashcardController(UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetFlashcards()
    {
        var userId = _userManager.GetUserId(User);
        var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Flashcards", userId + ".json");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { Success = false, Message = "No flashcards found!" });
        }

        var existingJson = await System.IO.File.ReadAllTextAsync(filePath);
        var flashcardsList = JsonSerializer.Deserialize<List<object>>(existingJson) ?? new List<object>();

        return Ok(new { flashcards = flashcardsList });
    }
    
    [Authorize]
    [HttpDelete("")]
    public async Task<IActionResult> DeleteFlashcard([FromBody] FlashcardModel flashcard)
    {
        var userId = _userManager.GetUserId(User);
        string filePath = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Flashcards", userId + ".json");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Flashcards", userId + ".json");
        }

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { Success = false, Message = "No flashcards found!" });
        }

        var existingJson = await System.IO.File.ReadAllTextAsync(filePath);
        var flashcardsList = JsonSerializer.Deserialize<List<FlashcardModel>>(existingJson) ?? new List<FlashcardModel>();

        var flashcardExists = flashcardsList.RemoveAll(f =>
            f.Title == flashcard.Title && f.Question == flashcard.Question && f.Answer == flashcard.Answer) > 0;

        if (flashcardExists)
        {
            var updatedJson = JsonSerializer.Serialize(flashcardsList, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);
        }

        return Ok(new { Success = flashcardExists, Message = flashcardExists ? "Flashcard deleted successfully" : "Flashcard not found" });
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> SaveFlashcard([FromBody] FlashcardModel flashcard)
    {
        var userId = _userManager.GetUserId(User);
        var newFlashcard = new { title = flashcard.Title, question = flashcard.Question, answer = flashcard.Answer };
        string filePath = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Flashcards", userId + ".json");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Flashcards", userId + ".json");
        }

        List<object> flashcardsList = new List<object>();

        // If the file exists, read the JSON and deserialize it into a list of objects
        if (System.IO.File.Exists(filePath))
        {
            var existingJson = await System.IO.File.ReadAllTextAsync(filePath);
            flashcardsList = JsonSerializer.Deserialize<List<object>>(existingJson) ?? new List<object>();
        }
        // Else create the directory if it does not exist and add the JSON file path to the database
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Flashcards")))
                {
                    Directory.CreateDirectory(
                        Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/Flashcards"));
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\Flashcards")))
                {
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath,
                        "Data\\Files\\Flashcards"));
                }
            }
            
            // add record to UserCalendar table
            var userFlashcard = new Flashcard
            {
                UserId = userId,
                JSONPath = filePath
            };
            _context.Flashcards.Add(userFlashcard);
            
            // save changes
            await _context.SaveChangesAsync();
        }

        // Check if the flashcard already exists in the list
        bool flashcardExists = flashcardsList.Any(f => 
            JsonSerializer.Serialize(f) == JsonSerializer.Serialize(newFlashcard));

        if (!flashcardExists)
        {
            flashcardsList.Add(newFlashcard);
            var updatedJson = JsonSerializer.Serialize(flashcardsList, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);
        }

        return Ok(new { Success = !flashcardExists, Message = flashcardExists ? "Flashcard already exists" : "Flashcard saved successfully" });
    }
}