using DuckI.Data;
using DuckI.Dtos;
using DuckI.Models;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

/// <summary>
/// This class provides services for managing role management.
/// </summary>
/// <remarks>
/// This service includes methods for applying to roles and getting all records from the table.
/// </remarks>
public interface IUserRoleStatusesService
{
    Task AddUserRoleStatusAsync(string userId, string roleId, string description);
    Task<IEnumerable<UserRoleStatusDto>> GetAllUserRoleStatusesAsync();
}

/// <summary>
/// This class implements services for managing calendars.
/// </summary>
public class UserRoleStatusesService : IUserRoleStatusesService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;
    
    public UserRoleStatusesService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    /// <summary>
    /// Add user, role, status and description to the table UserRoleStatuses
    /// </summary>
    /// <param name="userId">id of the User</param>
    /// <param name="roleName">role for which we are applying, string representation</param>
    /// <param name="description">string description</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task AddUserRoleStatusAsync(string userId, string roleName, string description)
    {
        // find the roleId
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        var roleId = role.Id;
        
        var existingUserRoleStatus = await _context.UserRoleStatuses
            .FirstOrDefaultAsync(urs => urs.UserId == userId); // check if UserRoleStatus record for this user already exists
        
        // if it does exist we want to prevent adding the same user in the table again (note that status is either approved or pending)
        // if it doesn't exist, we can add the user to the table
        if (existingUserRoleStatus != null)
        {
            throw new InvalidOperationException("UserRoleStatus for this user already exists.");
        }
        
        var userRoleStatus = new UserRoleStatus
        {
            UserId = userId,
            RoleId = roleId,
            Status = false, // true - approved, false - pending
            Description = description
        };
        
        _context.UserRoleStatuses.Add(userRoleStatus);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all records from the UserRoleStatuses table, uses UserRoleStatusDto to return the data
    /// </summary>
    public async Task<IEnumerable<UserRoleStatusDto>> GetAllUserRoleStatusesAsync()
    {
        return await _context.UserRoleStatuses
            .Select(urs => new UserRoleStatusDto
            {
                UserId = urs.UserId,
                RoleId = urs.RoleId,
                Status = urs.Status,
                Description = urs.Description
            })
            .ToListAsync();
    }
}