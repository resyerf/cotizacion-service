using Cotizacion.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace Cotizacion.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest,
                "Error de validación.",
                ve.Errors.Select(e => e.ErrorMessage).ToArray()),

            DomainException de => (StatusCodes.Status422UnprocessableEntity, de.Message, Array.Empty<string>()),

            NotFoundException nfe => (StatusCodes.Status404NotFound, nfe.Message, Array.Empty<string>()),

            _ => (StatusCodes.Status500InternalServerError, "Ha ocurrido un error interno.", Array.Empty<string>())
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new { message, errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
