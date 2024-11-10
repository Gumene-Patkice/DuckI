using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class GlobalErrorHandeler
{
    private readonly RequestDelegate _next;

    public GlobalErrorHandeler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Call the next middleware
        }
        catch (Exception ex)
        {
            // Handle the exception and redirect as needed
            context.Response.Redirect("/Home/UploadCalendar");
        }
    }
}