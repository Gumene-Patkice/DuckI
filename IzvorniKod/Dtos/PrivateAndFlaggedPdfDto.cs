namespace DuckI.Dtos;

public class PrivateAndFlaggedPdfDto
{
    public long PdfId { get; set; }
    public string PdfPath { get; set; }
    public string PdfName { get; set; }
    public int Rating { get; set; }
    public int TagId { get; set; }
    public string TagName { get; set; }
    public bool IsPublic { get; set; }
    public string EducatorUsername { get; set; }
}