// M3alam.Web/Middleware/ExceptionMiddleware.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);  // Proceed to the next middleware
        }
        catch (Exception ex)  // Catch any unhandled exceptions
        {
            await HandleExceptionAsync(httpContext, ex);  // Handle the exception
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            message = "Internal server error",
            details = exception.Message  // Include the exception message in the response
        };

        return context.Response.WriteAsJsonAsync(response);  // Send error response
    }
}
