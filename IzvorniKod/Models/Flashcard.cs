using Microsoft.AspNetCore.Identity;

namespace DuckI.Models;

public class Flashcard
{
    public string UserId { get; set; }
    public string JSONPath { get; set; }
    public IdentityUser User { get; set; }
}