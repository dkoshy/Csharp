using EventScheduler.Services.Exceptions;
using System.Text.Json;

namespace EventScheduler.MiddlleWares;

/// <summary>
/// 
/// </summary>

public class CustomeExceptionMiddleWare 
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IEventServiceException> _logger;

   /// <summary>
   /// 
   /// </summary>
   /// <param name="next"></param>
   /// <param name="logger"></param>
    public CustomeExceptionMiddleWare(RequestDelegate next
        , ILogger<IEventServiceException> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        } 
        catch (Exception ex)
        {
            await HandleExceptionAsync(context , ex);
        }

    }

    private async Task HandleExceptionAsync(HttpContext context
        , Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

       if(ex != null)
        {
            if(ex is IEventServiceException exception)
            {
                context.Response.StatusCode = (int)exception.StatusCode;

                await context.Response.WriteAsync(exception.ToJson());
                _logger?.LogError(exception.EventId, ex, ex.Message);
            }
            else
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(new 
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Unknown",
                    TraceId = context.TraceIdentifier
                },
                new JsonSerializerOptions(JsonSerializerDefaults.Web)));

                _logger?.LogError(new EventId(0,"Unknown"), ex, ex.Message);
            }
        }

    }
}
