using DuckI.Data;
using DuckI.Dtos;
using DuckI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

public interface IManagePdfService
{
    Task<List<PublicPdfDto>> GetAllPublicPdfsAsync();
    Task<List<PrivateAndFlaggedPdfDto>> GetUserPrivateAndFlaggedPdfsAsync(string userId);
    Task<List<PrivateAndFlaggedPdfDto>> GetUserEducatorsPdfs(string userId);
    Task AddUserToFlaggedPdfsAsync(string userId, long publicPdfId);
    Task RemoveUserFromFlaggedPdfsAsync(string userId, long publicPdfId);
    Task<string> GetPdfPathByIdAsync(long pdfId, string isPublic);
}

public class ManagePdfService : IManagePdfService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public ManagePdfService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<List<PublicPdfDto>> GetAllPublicPdfsAsync()
    {
        return await _context.PublicPdfs
            .Include(pdf => pdf.PublicPdfTag)
            .ThenInclude(tag => tag.Tag)
            .Include(pdf => pdf.EducatorPdf)
            .ThenInclude(ep => ep.User)
            .Select(pdf => new PublicPdfDto
            {
                PublicPdfId = pdf.PublicPdfId,
                PdfPath = pdf.PdfPath,
                PdfName = pdf.PdfName,
                Rating = pdf.Rating,
                TagName = pdf.PublicPdfTag.Tag.TagName,
                EducatorUsername = pdf.EducatorPdf.User.UserName
            })
            .ToListAsync();
    }

    public async Task<List<PrivateAndFlaggedPdfDto>> GetUserPrivateAndFlaggedPdfsAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new List<PrivateAndFlaggedPdfDto>();
        }
        
        var privatePdfs = await _context.StudentPdfs
            .Where(sp => sp.UserId == userId)
            .Include(sp => sp.PrivatePdf)
            .ThenInclude(pdf => pdf.PrivatePdfTag)
            .ThenInclude(tag => tag.Tag)
            .Select(sp => new PrivateAndFlaggedPdfDto
            {
                PdfId = sp.PrivatePdf.PrivatePdfId,
                PdfPath = sp.PrivatePdf.PdfPath,
                PdfName = sp.PrivatePdf.PdfName,
                Rating = 0,
                TagName = sp.PrivatePdf.PrivatePdfTag.Tag.TagName,
                IsPublic = false
            })
            .ToListAsync();

        var flaggedPdfs = await _context.FlaggedPdfs
            .Where(fp => fp.UserId == userId)
            .Include(fp => fp.PublicPdf)
            .ThenInclude(pdf => pdf.PublicPdfTag)
            .ThenInclude(tag => tag.Tag)
            .Include(fp => fp.PublicPdf)
            .ThenInclude(pdf => pdf.EducatorPdf)
            .ThenInclude(ep => ep.User)
            .Select(fp => new PrivateAndFlaggedPdfDto
            {
                PdfId = fp.PublicPdf.PublicPdfId,
                PdfPath = fp.PublicPdf.PdfPath,
                PdfName = fp.PublicPdf.PdfName,
                Rating = fp.PublicPdf.Rating,
                TagName = fp.PublicPdf.PublicPdfTag.Tag.TagName,
                IsPublic = true,
                EducatorUsername = fp.PublicPdf.EducatorPdf.User.UserName
            })
            .ToListAsync();

        return privatePdfs.Concat(flaggedPdfs).ToList();
    }

    public async Task<List<PrivateAndFlaggedPdfDto>> GetUserEducatorsPdfs(string userId)
    {
        var educatorPdfs = await _context.EducatorPdfs
            .Where(sp => sp.UserId == userId)
            .Include(sp => sp.PublicPdf)
            .ThenInclude(pdf => pdf.PublicPdfTag)
            .ThenInclude(tag => tag.Tag)
            .Select(sp => new PrivateAndFlaggedPdfDto
            {
                PdfId = sp.PublicPdf.PublicPdfId,
                PdfPath = sp.PublicPdf.PdfPath,
                PdfName = sp.PublicPdf.PdfName,
                Rating = sp.PublicPdf.Rating,
                TagName = sp.PublicPdf.PublicPdfTag.Tag.TagName,
                IsPublic = false
            })
            .ToListAsync();

        return educatorPdfs;
    }
    
    public async Task AddUserToFlaggedPdfsAsync(string userId, long publicPdfId)
    {
        var existingFlaggedPdf = await _context.FlaggedPdfs
            .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.PublicPdfId == publicPdfId);

        if (existingFlaggedPdf == null)
        {
            var flaggedPdf = new FlaggedPdf
            {
                UserId = userId,
                PublicPdfId = publicPdfId
            };

            _context.FlaggedPdfs.Add(flaggedPdf);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("User already flagged this pdf");
        }
    }
    
    public async Task RemoveUserFromFlaggedPdfsAsync(string userId, long publicPdfId)
    {
        var flaggedPdf = await _context.FlaggedPdfs
            .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.PublicPdfId == publicPdfId);

        if (flaggedPdf != null)
        {
            _context.FlaggedPdfs.Remove(flaggedPdf);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("User has not flagged this pdf!");
        }
    }

    public async Task<string> GetPdfPathByIdAsync(long pdfId, string isPublic)
    {
        if (isPublic == "true")
        {
            var pdf = await _context.PublicPdfs
                .FirstOrDefaultAsync(p => p.PublicPdfId == pdfId);

            return pdf?.PdfPath;
        }
        else if (isPublic == "false")
        {
            var pdf = await _context.PrivatePdfs
                .FirstOrDefaultAsync(p => p.PrivatePdfId == pdfId);

            return pdf?.PdfPath;
        }
        else
        {
            return null;
        }
    }
}