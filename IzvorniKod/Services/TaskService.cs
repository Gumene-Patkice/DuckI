using DuckI.Data;
using DuckI.Dtos;

namespace DuckI.Services;

public interface ITaskService
{
    Task<PdfsForTaskDto> CreateTask(long pdfId, bool isPublic);
}

public class TaskService : ITaskService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;
    
    public TaskService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }
    
    public async Task<PdfsForTaskDto> CreateTask(long pdfId, bool isPublic)
    {
        if (isPublic)
        {
            var publicPdf = await _context.PublicPdfs.FindAsync(pdfId);
            if (publicPdf == null)
            {
                throw new InvalidOperationException("Public PDF not found.");
            }
            return new PdfsForTaskDto
            {
                PdfId = publicPdf.PublicPdfId,
                PdfPath = publicPdf.PdfPath,
                PdfName = publicPdf.PdfName
            };
        }
        else
        {
            var privatePdf = await _context.PrivatePdfs.FindAsync(pdfId);
            if (privatePdf == null)
            {
                throw new InvalidOperationException("Private PDF not found.");
            }
            return new PdfsForTaskDto
            {
                PdfId = privatePdf.PrivatePdfId,
                PdfPath = privatePdf.PdfPath,
                PdfName = privatePdf.PdfName
            };
        }
    }
}