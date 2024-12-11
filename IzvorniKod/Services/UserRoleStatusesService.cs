using DuckI.Data;
using DuckI.Dtos;
using DuckI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Services;

/// <summary>
/// This class provides services for managing role management.
/// </summary>
/// <remarks>
/// This service includes methods for applying to roles and getting all records from the table,
/// and managing roles using UserRoleStatuses table.
/// </remarks>
public interface IUserRoleStatusesService
{
    Task AddUserRoleStatusAsync(string userId, string roleId, string description);
    Task<IEnumerable<UserRoleStatusDto>> GetAllUserRoleStatusesAsync();
    Task<UserRoleStatusDto> GetUserRoleStatusByUserIdAsync(string userId);
    Task AddUserToRoleAsync(string userId, string roleId);
    Task RejectUserAsync(string userId, string roleId);
    Task AssignSuperStudentAsync(string userId, string roleId);
}

/// <summary>
/// This class implements services for managing calendars.
/// </summary>
public class UserRoleStatusesService : IUserRoleStatusesService
{
    private readonly IWebHostEnvironment _webHostEnvironment; // for accessing directory path and other webhostenv functionalities
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public UserRoleStatusesService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
        _userManager = userManager;
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

    /// <summary>
    /// Retrieves a single UserRoleStatuses record for the user who called the controller's route
    /// </summary>
    public async Task<UserRoleStatusDto> GetUserRoleStatusByUserIdAsync(string userId)
    {
        var userRoleStatus = await _context.UserRoleStatuses
            .FirstOrDefaultAsync(urs => urs.UserId == userId);
        
        if (userRoleStatus == null)
        {
            return null;
        }
        
        return new UserRoleStatusDto
        {
            UserId = userRoleStatus.UserId,
            RoleId = userRoleStatus.RoleId,
            Status = userRoleStatus.Status,
            Description = userRoleStatus.Description
        };
    }

    /// <summary>
    /// Adds a user to a role and updates the status in the UserRoleStatuses table
    /// </summary>
    public async Task AddUserToRoleAsync(string userId, string roleId)
    {
        // find the user and check if it exists
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        // do the same for role
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found.");
        }

        // get UserRoleStatuses record for this user
        var userRoleStatus = await _context.UserRoleStatuses
            .FirstOrDefaultAsync(urs => urs.UserId == userId && urs.RoleId == roleId);

        // if the user has already been approved, we don't want to add the user to the table again
        if (userRoleStatus != null && userRoleStatus.Status)
        {
            throw new InvalidOperationException("User has already been approved!");
        }
        
        // add user to role
        var result = await _userManager.AddToRoleAsync(user, role.Name);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to add user to role.");
        }
        
        if (userRoleStatus != null)
        {
            userRoleStatus.Status = true; // Update status to approved
            _context.UserRoleStatuses.Update(userRoleStatus);
        }
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove UserRoleStatuses record from UserRoleStatuses table
    /// </summary>
    public async Task RejectUserAsync(string userId, string roleId)
    {
        // get record for the user
        var userRoleStatus = await _context.UserRoleStatuses
            .FirstOrDefaultAsync(urs => urs.UserId == userId && urs.RoleId == roleId);

        if (userRoleStatus == null)
        {
            throw new InvalidOperationException("UserRoleStatus not found.");
        }
        
        // if the user has already been approved, we don't want to reject the user to the table again
        if (userRoleStatus.Status)
        {
            throw new InvalidOperationException("User has already been approved!");
        }

        // remove the record
        _context.UserRoleStatuses.Remove(userRoleStatus);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Assigns a user to a role and updates the status in the UserRoleStatuses table 
    /// </summary>
    public async Task AssignSuperStudentAsync(string userId, string roleName)
    {
        // find the user and check if it exists
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        // do the same for role
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found.");
        }
        
        var roleId = role.Id;
        
        // get UserRoleStatuses record for this user
        var userRoleStatus = await _context.UserRoleStatuses
            .FirstOrDefaultAsync(urs => urs.UserId == userId);

        // if the user has already applied for a role, we don't want to add the user to the table again
        if (userRoleStatus != null)
        {
            throw new InvalidOperationException("User has already applied for a role!");
        }
        
        // add user to role
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to add user to the role.");
        }

        // add user to UserRoleStatuses table with status approved
        var userRoleStatusRecord = new UserRoleStatus
        {
            UserId = userId,
            Description = "SuperStudent",
            RoleId = roleId,
            Status = true
        };

        _context.UserRoleStatuses.Add(userRoleStatusRecord);
        await _context.SaveChangesAsync();
    }
}