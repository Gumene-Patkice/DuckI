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
    public DbSet<FlaggedPdf> FlaggedPdfs { get; set; }
    public DbSet<RatingLog> RatingLogs { get; set; }
    public DbSet<RemovedLog> RemovedLogs { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    
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
        
        // one to many; each User can have many records in FlaggedPdf
        modelBuilder.Entity<FlaggedPdf>()
            .HasKey(fp => new { fp.UserId, fp.PublicPdfId });

        modelBuilder.Entity<FlaggedPdf>()
            .HasOne(fp => fp.User)
            .WithMany()
            .HasForeignKey(fp => fp.UserId);

        modelBuilder.Entity<FlaggedPdf>()
            .HasOne(fp => fp.PublicPdf)
            .WithMany()
            .HasForeignKey(fp => fp.PublicPdfId);
        
        // one to many; each Student can rate multiple pdfs
        // each public pdf can be rated by multiple students
        modelBuilder.Entity<RatingLog>()
            .HasKey(rl => new { rl.UserId, rl.PublicPdfId });

        modelBuilder.Entity<RatingLog>()
            .HasOne(rl => rl.User)
            .WithMany()
            .HasForeignKey(rl => rl.UserId);

        modelBuilder.Entity<RatingLog>()
            .HasOne(rl => rl.PublicPdf)
            .WithMany()
            .HasForeignKey(rl => rl.PublicPdfId);
        
        // one to many; each Reviewer can have many records in RemovedLog
        // each Educator can have many records in RemovedLog
        modelBuilder.Entity<RemovedLog>()
            .HasKey(rl => rl.RemoveLogId);

        modelBuilder.Entity<RemovedLog>()
            .HasOne(rl => rl.Reviewer)
            .WithMany()
            .HasForeignKey(rl => rl.ReviewerId);

        modelBuilder.Entity<RemovedLog>()
            .HasOne(rl => rl.Educator)
            .WithMany()
            .HasForeignKey(rl => rl.EducatorId);
        
        // one to one; each user has a JSON file of flashcards
        modelBuilder.Entity<Flashcard>()
            .HasKey(f => f.UserId);
        modelBuilder.Entity<Flashcard>()
            .HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId);
    }
}