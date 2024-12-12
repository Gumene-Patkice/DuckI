using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class StudentPdf
{
    public string UserId { get; set; }
    public long PrivatePdfId { get; set; }
    public IdentityUser User { get; set; }
    public PrivatePdf PrivatePdf { get; set; }
}