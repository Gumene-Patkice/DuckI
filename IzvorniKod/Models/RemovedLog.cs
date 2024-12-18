using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class RemovedLog
{
    public long RemoveLogId { get; set; }
    public string ReviewerId { get; set; }
    public string EducatorId { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public IdentityUser Reviewer { get; set; }
    public IdentityUser Educator { get; set; }
}