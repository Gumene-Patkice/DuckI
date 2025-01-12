using System.Text;
using DuckI.Data;
using Mscc.GenerativeAI;
using System.Text.Json;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DuckI.Services;

public interface ITaskService
{
    Task<string?> CreateTask(long pdfId, bool isPublic);
}

public class TasksService : ITaskService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;
    private readonly string _apiKey;

    public TasksService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IConfiguration configuration)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
        _apiKey = configuration["Key:Gemini"];
    }

    public async Task<string?> CreateTask(long pdfId, bool isPublic)
    {
        PdfReader reader = null;
        if (isPublic)
        {
            var publicPdf = await _context.PublicPdfs.FindAsync(pdfId);
            if (publicPdf == null)
            {
                throw new InvalidOperationException("Public PDF not found.");
            }
            reader = new PdfReader(publicPdf.PdfPath);
        }
        else
        {
            var privatePdf = await _context.PrivatePdfs.FindAsync(pdfId);
            if (privatePdf == null)
            {
                throw new InvalidOperationException("Private PDF not found.");
            }

            reader = new PdfReader(privatePdf.PdfPath);
        }
        
        if (reader == null)
        {
            throw new InvalidOperationException("PDF not found.");
        }
        
        string text = string.Empty;
        for (int page = 1; page <= reader.NumberOfPages; page++)
        {
            ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
            string strText = PdfTextExtractor.GetTextFromPage(reader, page, its);
            //strText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(strText)));
            text += strText;
        }
        reader.Close();
        
        var prompt = @"
                    You are a helpful AI assistant that generates flashcards in JSON format.
                    Your response must be a valid JSON array with objects, each having 'title' (same title for all the flashcards based on the provided content), 'question' and 'answer' properties.
                    The response must be in the Croatian language.

                    Example:
                    [
                      { 'title': 'Povijest francuske revolucije', 'question': 'Koji se događaj iz 1789. godine smatra početkom Francuske revolucije?', 'answer': 'Juriš na Bastilju' },
                      { 'title': 'Povijest francuske revolucije', 'question': 'U kojoj bitki je Napoleon konačno poražen 1815.?', 'answer': 'Bitka kod Waterlooa' }
                    ]

                    Generate flashcards from this PDF content: 
                    ".Replace("'", "\"");
        prompt += text;
            
        
        IGenerativeAI genAi = new GoogleAI(_apiKey);
        var model = genAi.GenerativeModel(Model.Gemini15Flash);
        var generatorConfig = new GenerationConfig
        {
            ResponseMimeType = "application/json"
        };
        var response = await model.GenerateContent(prompt, generatorConfig);
        // Parse the response.Text JSON array
        var flashcardsArray = JsonSerializer.Deserialize<JsonElement>(response.Text);

        // Create a new JSON object with a Flashcards property
        var wrappedResponse = new
        {
            Flashcards = flashcardsArray
        };

        // Serialize the new JSON object back to a string
        var wrappedResponseJson = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        return wrappedResponseJson;
    }
}