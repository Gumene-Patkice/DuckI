using DuckI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<UserCalendar> UserCalendars { get; set; }
    public DbSet<UserRoleStatus> UserRoleStatuses { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserCalendar>()
            .HasKey(uc => new { uc.UserId, uc.CalendarId });

        modelBuilder.Entity<UserCalendar>()
            .HasOne(uc => uc.User)
            .WithOne()
            .HasForeignKey<UserCalendar>(uc => uc.UserId);

        modelBuilder.Entity<UserCalendar>()
            .HasOne(uc => uc.Calendar)
            .WithOne(c => c.UserCalendar)
            .HasForeignKey<UserCalendar>(uc => uc.CalendarId);
        
        modelBuilder.Entity<UserRoleStatus>()
            .HasKey(urs => urs.UserId);
        
        // one to one; user can have only one record at a time in this table
        // each record in UserRoleStatuses is associated with one user
        modelBuilder.Entity<UserRoleStatus>()
            .HasOne(urs => urs.User)
            .WithOne()
            .HasForeignKey<UserRoleStatus>(urs => urs.UserId);
        
        // one to many; each role can have many records in UserRoleStatuses
        // while each (user) record is associated with only role
        modelBuilder.Entity<UserRoleStatus>()
            .HasOne(urs => urs.Role)
            .WithMany()
            .HasForeignKey(urs => urs.RoleId);
    }
}