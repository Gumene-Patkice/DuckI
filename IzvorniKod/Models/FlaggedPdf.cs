using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class FlaggedPdf
{
    public string UserId { get; set; }
    public long PublicPdfId { get; set; }
    public IdentityUser User { get; set; }
    public PublicPdf PublicPdf { get; set; }
}