using DuckI.Data;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

public interface ITagService
{
    Task AddTagAsync(string tagName);
    Task<List<Tag>> GetAllTagsAsync();
}

public class TagService : ITagService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;

    public TagService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    public async Task AddTagAsync(string tagName)
    {
        var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
        if (existingTag != null)
        {
            throw new InvalidOperationException("A tag with this name already exists!");
        }

        // If the tag does not exist, add the new tag
        var tag = new Tag { TagName = tagName };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags.ToListAsync();
    }
}