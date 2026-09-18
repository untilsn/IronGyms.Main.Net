using IronGyms.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace IronGyms.Api.Middleware;

// Bắt MỌI exception ném ra từ Controller/Service, không cần try/catch lặp lại ở từng action.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            ApiException ex => (ex.StatusCode, ex.Message),

            // Exception lạ, không lường trước - không bao giờ lộ ex.Message thật ra ngoài
            // (có thể chứa thông tin nhạy cảm như connection string, stack trace...).
            _ => (StatusCodes.Status500InternalServerError, "Đã có lỗi xảy ra, vui lòng thử lại sau")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception tại {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new { message }, cancellationToken);

        return true;
    }
}