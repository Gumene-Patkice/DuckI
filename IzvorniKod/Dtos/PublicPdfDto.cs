namespace DuckI.Dtos;

public class PublicPdfDto
{
    public long PublicPdfId { get; set; }
    public string PdfPath { get; set; }
    public string PdfName { get; set; }
    public int Rating { get; set; }
    public int TagId { get; set; }
    public string TagName { get; set; }
    public string EducatorUsername { get; set; }
}