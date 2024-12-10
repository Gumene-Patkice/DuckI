namespace DuckI.Models;

public class PublicPdf
{
    public long PublicPdfId { get; set; }
    public string PdfPath { get; set; }
    public string PdfName { get; set; }
    public int Rating { get; set; }
    public PublicPdfTag PublicPdfTag { get; set; }
    public EducatorPdf EducatorPdf { get; set; }
}