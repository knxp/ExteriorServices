using System.Text.Json;
using ExteriorServices.Api.Errors;

namespace ExteriorServices.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger)
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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
        }
        catch (VisualizationValidationException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                new ApiError("VisualizationValidationError", exception.Message));
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled API exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                new ApiError(
                    "UnexpectedError",
                    "An unexpected error occurred while processing the request."));
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        ApiError error)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}