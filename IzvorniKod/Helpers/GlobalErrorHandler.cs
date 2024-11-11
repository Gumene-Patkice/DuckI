using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DuckI.Exceptions;
using DuckI.Helpers;

public class GlobalErrorHandler
{
    private readonly RequestDelegate _next;

    public GlobalErrorHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            await HandleExceptionAs(context, ex);
        }
    }

    private async Task HandleExceptionAs(HttpContext context, Exception exception)
    {
        ExceptionResponse response;
        switch (exception)
        {
            case WrongFileFormatException:
                response = new ExceptionResponse(HttpStatusCode.Forbidden, exception.Message);
                break;
            default:
                response = new ExceptionResponse(HttpStatusCode.InternalServerError, exception.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.Status;
        await context.Response.WriteAsync(response.ToJson());
    }
} 