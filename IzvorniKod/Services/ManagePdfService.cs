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
    Task RatePdfAsync(long pdfId, string userId, bool isUpvote);
    Task DeletePrivatePdfAsync(long pdfId, string userId);
    Task DeletePublicPdfAsync(long pdfId, string userId);
    Task DeletePublicPdfReviewerAsync(long pdfId, string  reviewerId, string description);
    Task<List<RemovedLog>> GetAllRemovedLogsAsync(string educatorId);
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

    public async Task RatePdfAsync(long pdfId, string userId, bool isUpvote)
    {
        var existingRatingLog = await _context.RatingLogs
            .FirstOrDefaultAsync(rl => rl.PublicPdfId == pdfId && rl.UserId == userId);

        if (existingRatingLog == null)
        {
            var ratingLog = new RatingLog
            {
                UserId = userId,
                PublicPdfId = pdfId
            };

            _context.RatingLogs.Add(ratingLog);
            
            var publicPdf = await _context.PublicPdfs
                .FirstOrDefaultAsync(pdf => pdf.PublicPdfId == pdfId);
            
            publicPdf.Rating += isUpvote ? 1 : -1;
            _context.PublicPdfs.Update(publicPdf);
            
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task DeletePrivatePdfAsync(long pdfId, string userId)
    {
        var privatePdf = await _context.PrivatePdfs
            .Include(p => p.PrivatePdfTag)
            .Include(p => p.StudentPdf)
            .FirstOrDefaultAsync(p => p.PrivatePdfId == pdfId);

        if (privatePdf == null)
        {
            throw new InvalidOperationException("Private PDF not found.");
        }
        
        if(privatePdf.StudentPdf.UserId != userId)
        {
            throw new InvalidOperationException("You are not authorized to delete this PDF.");
        }
        
        File.Delete(privatePdf.PdfPath);
        
        if (privatePdf.PrivatePdfTag != null)
        {
            _context.PrivatePdfTags.Remove(privatePdf.PrivatePdfTag);
        }

        if (privatePdf.StudentPdf != null)
        {
            _context.StudentPdfs.Remove(privatePdf.StudentPdf);
        }

        _context.PrivatePdfs.Remove(privatePdf);

        await _context.SaveChangesAsync();
        
        
    }

    public async Task DeletePublicPdfAsync(long pdfId, string userId)
    {
        var publicPdf = await _context.PublicPdfs
            .Include(p => p.PublicPdfTag)
            .Include(p => p.EducatorPdf)
            .FirstOrDefaultAsync(p => p.PublicPdfId == pdfId);

        if (publicPdf == null)
        {
            throw new InvalidOperationException("Public PDF not found.");
        }

        if (publicPdf.EducatorPdf.UserId != userId)
        {
            throw new InvalidOperationException("You are not authorized to delete this PDF.");
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

        var ratingLogs = _context.RatingLogs.Where(rl => rl.PublicPdfId == pdfId);
        _context.RatingLogs.RemoveRange(ratingLogs);

        var flaggedPdfs = _context.FlaggedPdfs.Where(fp => fp.PublicPdfId == pdfId);
        _context.FlaggedPdfs.RemoveRange(flaggedPdfs);

        _context.PublicPdfs.Remove(publicPdf);

        await _context.SaveChangesAsync();
    }

    public async Task DeletePublicPdfReviewerAsync(long pdfId, string reviewerId, string description)
    {
        var publicPdf = await _context.PublicPdfs
            .Include(p => p.PublicPdfTag)
            .Include(p => p.EducatorPdf)
            .FirstOrDefaultAsync(p => p.PublicPdfId == pdfId);

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

        var ratingLogs = _context.RatingLogs.Where(rl => rl.PublicPdfId == pdfId);
        _context.RatingLogs.RemoveRange(ratingLogs);

        var flaggedPdfs = _context.FlaggedPdfs.Where(fp => fp.PublicPdfId == pdfId);
        _context.FlaggedPdfs.RemoveRange(flaggedPdfs);

        _context.PublicPdfs.Remove(publicPdf);
        
        var removedLog = new RemovedLog
        {
            ReviewerId = reviewerId,
            EducatorId = publicPdf.EducatorPdf.UserId,
            Description = description,
            FileName = publicPdf.PdfName
        };

        _context.RemovedLogs.Add(removedLog);

        await _context.SaveChangesAsync();
    }
    
    public async Task<List<RemovedLog>> GetAllRemovedLogsAsync(string educatorId)
    {
        return await _context.RemovedLogs
            .Include(rl => rl.Reviewer)
            .Include(rl => rl.Educator)
            .Where(rl => rl.EducatorId == educatorId)
            .ToListAsync();
    }
}