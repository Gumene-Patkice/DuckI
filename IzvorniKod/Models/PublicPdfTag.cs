namespace DuckI.Models;

public class PublicPdfTag
{
    public long PublicPdfId { get; set; }
    public int TagId { get; set; }
    public  PublicPdf PublicPdf { get; set; }
    public Tag Tag { get; set; }
}