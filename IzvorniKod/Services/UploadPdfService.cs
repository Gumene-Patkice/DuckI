using System.Runtime.InteropServices;
using DuckI.Data;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

/// <summary>
/// This class provides services for managing uploads for pdfs.
/// </summary>
public interface IUploadPdfService
{
    Task UploadPrivatePdfAsync(IFormFile file, string userId, string tagName);
    Task UploadPublicPdfAsync(IFormFile file, string userId, string tagName);
}
    
/// <summary>
/// This service is used for uploading PDFs by both educators and students (SuperStudents).
/// </summary>
public class UploadPdfService : IUploadPdfService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;

    public UploadPdfService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }
    
    /// <summary>
    /// This function is used in PdfController.cs for uploading private PDFs by students (SuperStudents).
    /// </summary>
    public async Task UploadPrivatePdfAsync(IFormFile file, string userId, string tagName)
    {
        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only PDF files are allowed.");
        }
        
        var fileName = $"{userId}${file.FileName}";

        var filePath = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/PrivatePdfs", fileName);    
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\PrivatePdfs", fileName);
        }
        
        var existingPdf = await _context.PrivatePdfs.FirstOrDefaultAsync(p => p.PdfPath == filePath);
        
        if (existingPdf != null) 
        { 
            // Replace the existing PDF
            using (var stream = new FileStream(filePath, FileMode.Create)) 
            { 
                await file.CopyToAsync(stream);
            }
            
            // Update the tag if it is different
            var existingTag = await _context.PrivatePdfTags.FirstOrDefaultAsync(pt => pt.PrivatePdfId == existingPdf.PrivatePdfId);
            var newTag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
            if (newTag == null)
            {
                throw new InvalidOperationException("That tag doesn't exist!");
            }

            if (existingTag.TagId != newTag.TagId)
            {
                existingTag.TagId = newTag.TagId;
                _context.PrivatePdfTags.Update(existingTag);
                await _context.SaveChangesAsync();
            }
        }
        else 
        { 
            // Create new file
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/PrivatePdfs"))) 
                { 
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, 
                        "Data/Files/PrivatePdfs"));
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\PrivatePdfs"))) 
                { 
                    Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, 
                        "Data/Files/PrivatePdfs"));
                }
            }
            
            using (var stream = new FileStream(filePath, FileMode.Create)) 
            { 
                await file.CopyToAsync(stream);
            }

            // Update PrivatePdfs table
            var privatePdf = new PrivatePdf 
            { 
                PdfPath = filePath, 
                PdfName = file.FileName
            }; 
            
            _context.PrivatePdfs.Add(privatePdf);
            await _context.SaveChangesAsync(); 
            
            // Update StudentPdfs table
            var studentPdf = new StudentPdf 
            { 
                UserId = userId, 
                PrivatePdfId = privatePdf.PrivatePdfId
            }; 
            
            _context.StudentPdfs.Add(studentPdf); 
            await _context.SaveChangesAsync();

            // Update PrivatePdfTags table
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName); 
            if (tag == null)
            {
                throw new InvalidOperationException("That tag doesn't exists!");
            }
            
            var privatePdfTag = new PrivatePdfTag 
            { 
                TagId = tag.TagId,
                PrivatePdfId = privatePdf.PrivatePdfId
            }; 
            
            _context.PrivatePdfTags.Add(privatePdfTag); 
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// This function is used in PdfController.cs for uploading public PDFs by educators.
    /// </summary>
    public async Task UploadPublicPdfAsync(IFormFile file, string userId, string tagName)
    {
        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only PDF files are allowed.");
        }
        
        var fileName = $"{userId}${file.FileName}";

        var filePath = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/PublicPdfs", fileName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\PublicPdfs", fileName);
        }
        
        var existingPdf = await _context.PublicPdfs.FirstOrDefaultAsync(p => p.PdfPath == filePath);
        
        if (existingPdf != null) 
        {
            var publicPdf = await _context.PublicPdfs
                .Include(p => p.PublicPdfTag)
                .Include(p => p.EducatorPdf)
                .FirstOrDefaultAsync(p => p.PublicPdfId == existingPdf.PublicPdfId);

            if (publicPdf == null)
            {
                throw new InvalidOperationException("Public PDF not found.");
            }
        
            File.Delete(publicPdf.PdfPath);

            if (publicPdf.PublicPdfTag != null)
            {
                _context.PublicPdfTags.Remove(publicPdf.PublicPdfTag);
            }

            if (publicPdf.EducatorPdf != null)
            {
                _context.EducatorPdfs.Remove(publicPdf.EducatorPdf);
            }

            var ratingLogs = _context.RatingLogs.Where(rl => rl.PublicPdfId == existingPdf.PublicPdfId);
            _context.RatingLogs.RemoveRange(ratingLogs);

            var flaggedPdfs = _context.FlaggedPdfs.Where(fp => fp.PublicPdfId == existingPdf.PublicPdfId);
            _context.FlaggedPdfs.RemoveRange(flaggedPdfs);

            _context.PublicPdfs.Remove(publicPdf);

            await _context.SaveChangesAsync();
        }
        
        // Create new file
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
        { 
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Files/PublicPdfs"))) 
            { 
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, 
                    "Data/Files/PublicPdfs"));
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
        { 
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Data\\Files\\PublicPdfs"))) 
            { 
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, 
                    "Data\\Files\\PublicPdfs"));
            }
        }

        using (var stream = new FileStream(filePath, FileMode.Create)) 
        { 
            await file.CopyToAsync(stream);
        }

        // Update PublicPdfs table
        var publicPdfForUpload = new PublicPdf 
        { 
            PdfPath = filePath, 
            PdfName = file.FileName, 
            Rating = 0
        };
        
        _context.PublicPdfs.Add(publicPdfForUpload); 
        await _context.SaveChangesAsync();
        
        // Update EducatorPdfs table
        var educatorPdf = new EducatorPdf 
        { 
            UserId = userId, 
            PublicPdfId = publicPdfForUpload.PublicPdfId
        };
        
        _context.EducatorPdfs.Add(educatorPdf); 
        await _context.SaveChangesAsync();

        // Update PublicPdfTags table
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
        if (tag == null) 
        { 
            throw new InvalidOperationException("That tag doesn't exists!");
        }
        
        var publicPdfTag = new PublicPdfTag
        { 
            TagId = tag.TagId, 
            PublicPdfId = publicPdfForUpload.PublicPdfId
        };
        
        _context.PublicPdfTags.Add(publicPdfTag); 
        await _context.SaveChangesAsync();
    } 
}