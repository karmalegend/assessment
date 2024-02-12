using System.Text.Json;
using Domain.Exceptions;

namespace ApI.Middleware;

public class DomainExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public DomainExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex is BusinessRuleException exception)
                await HandleExceptionAsync(httpContext, exception);
            else
                throw;
        }
    }

    private Task HandleExceptionAsync(HttpContext context, BusinessRuleException exception)
    {
        // Log your exception here
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) exception.StatusCode;

        var result = JsonSerializer.Serialize(new {error = exception.Message});
        return context.Response.WriteAsync(result);
    }
}
