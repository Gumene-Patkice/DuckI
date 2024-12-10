using DuckI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuckI.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<UserCalendar> UserCalendars { get; set; }
    public DbSet<UserRoleStatus> UserRoleStatuses { get; set; }
    public DbSet<PrivatePdf> PrivatePdfs { get; set; }
    public DbSet<PublicPdf> PublicPdfs { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PrivatePdfTag> PrivatePdfTags { get; set; }
    public DbSet<PublicPdfTag> PublicPdfTags { get; set; }
    public DbSet<StudentPdf> StudentPdfs { get; set; }
    public DbSet<EducatorPdf> EducatorPdfs { get; set; }
    
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
        
        // one to many; each tag can have many records in PrivatePdfTags
        // while each private pdf can have only one tag
        modelBuilder.Entity<PrivatePdfTag>()
            .HasKey(ppt => ppt.PrivatePdfId);

        modelBuilder.Entity<PrivatePdfTag>()
            .HasOne(ppt => ppt.Tag)
            .WithMany()
            .HasForeignKey(ppt => ppt.TagId);
        
        modelBuilder.Entity<PrivatePdfTag>()
            .HasOne(ppt => ppt.PrivatePdf)
            .WithOne(pp => pp.PrivatePdfTag)
            .HasForeignKey<PrivatePdfTag>(ppt => ppt.PrivatePdfId);
        
        // one to many; each tag can have many records in PublicPdfTags
        // while each public pdf can have only one tag
        modelBuilder.Entity<PublicPdfTag>()
            .HasKey(ppt => ppt.PublicPdfId);

        modelBuilder.Entity<PublicPdfTag>()
            .HasOne(ppt => ppt.Tag)
            .WithMany()
            .HasForeignKey(ppt => ppt.TagId);
        
        modelBuilder.Entity<PublicPdfTag>()
            .HasOne(ppt => ppt.PublicPdf)
            .WithOne(pp => pp.PublicPdfTag)
            .HasForeignKey<PublicPdfTag>(ppt => ppt.PublicPdfId);
        
        // one to many; each Student can have many records in StudentPdf
        modelBuilder.Entity<StudentPdf>()
            .HasKey(sp => sp.PrivatePdfId);
        
        modelBuilder.Entity<StudentPdf>()
            .HasOne(sp => sp.User)
            .WithMany()
            .HasForeignKey(sp => sp.UserId);

        modelBuilder.Entity<StudentPdf>()
            .HasOne(sp => sp.PrivatePdf)
            .WithOne(pp => pp.StudentPdf)
            .HasForeignKey<StudentPdf>(sp => sp.PrivatePdfId);
        
        // one to many; each Educator can have many records in EducatortPdf
        modelBuilder.Entity<EducatorPdf>()
            .HasKey(ep => ep.PublicPdfId);
        
        modelBuilder.Entity<EducatorPdf>()
            .HasOne(ep => ep.User)
            .WithMany()
            .HasForeignKey(ep => ep.UserId);
        
        modelBuilder.Entity<EducatorPdf>()
            .HasOne(ep => ep.PublicPdf)
            .WithOne(pp => pp.EducatorPdf)
            .HasForeignKey<EducatorPdf>(ep => ep.PublicPdfId);
    }
}