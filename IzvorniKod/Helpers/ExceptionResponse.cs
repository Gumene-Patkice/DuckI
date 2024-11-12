using System.Net;

namespace DuckI.Helpers;

public class ExceptionResponse
{
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }

    public ExceptionResponse(HttpStatusCode status, string message)
    {
        Status = status;
        Message = message;
    }

    public string ToJson()
    {
        return $"{{\"status\": \"{Status}\", \"message\": \"{Message}\"}}";
    }
}