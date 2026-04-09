using System.Text.Json;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Errors);
        }
        catch (UnauthorizedException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string error,
        IEnumerable<string>? details = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.Headers.CacheControl = "no-store";

        var body = new
        {
            status = statusCode,
            error,
            details = details?.ToArray() ?? []
        };

        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
