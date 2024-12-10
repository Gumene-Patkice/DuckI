namespace DuckI.Models;

public class PrivatePdf
{
    public long PrivatePdfId { get; set; }
    public string PdfPath { get; set; }
    public string PdfName { get; set; }
    public PrivatePdfTag PrivatePdfTag { get; set; }
    public StudentPdf StudentPdf { get; set; }
}