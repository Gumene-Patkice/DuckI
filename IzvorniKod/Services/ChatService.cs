using DuckI.Data;
using Mscc.GenerativeAI;
using System.Text.Json;

namespace DuckI.Services;

public interface IChatService
{
    Task<string?> PromptAsync(string prompt);
}

public class ChatService : IChatService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;
    private readonly string _apiKey;

    public ChatService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
        _apiKey = configuration["Key:Gemini"];
    }

    public async Task<string?> PromptAsync(string prompt)
    {
        IGenerativeAI genAi = new GoogleAI(_apiKey);
        var model = genAi.GenerativeModel(Model.Gemini15Flash);
        var response = await model.GenerateContent(prompt);
        var jsonResponse = JsonSerializer.Serialize(new { Text = response.Text });
        return jsonResponse;
    }
}