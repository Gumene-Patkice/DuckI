using DuckI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<UserCalendar> UserCalendars { get; set; }
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
    }
}