namespace DuckI.Models;

public class PrivatePdfTag
{
    public long PrivatePdfId { get; set; }
    public int TagId { get; set; }
    public  PrivatePdf PrivatePdf { get; set; }
    public Tag Tag { get; set; }
}