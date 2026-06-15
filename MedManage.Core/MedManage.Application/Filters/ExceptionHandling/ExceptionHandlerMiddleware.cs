using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MedManage.Application.Exceptions;

namespace UserManagement.Application.Filters.ExceptionHadling;

/// <summary>
/// Middleware для обработки исключений в приложении.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Конструктор middleware.
    /// </summary>
    /// <param name="next">Следующий делегат запроса.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Обработка запроса с обработкой исключений.
    /// </summary>
    /// <param name="context">HTTP-контекст запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AnnouncementNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status401Unauthorized, "Unauthorized access.");
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Метод для обработки и отправки исключений.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var result = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(result);
    }
}