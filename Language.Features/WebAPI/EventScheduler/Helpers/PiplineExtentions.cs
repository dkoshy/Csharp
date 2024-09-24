using EventScheduler.MiddlleWares;
using EventScheduler.Services.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace EventScheduler.Helpers;

/// <summary>
/// 
/// </summary>
public static class PiplineExtentions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="_logger"></param>
    public static void UseCustomExceptionHandler(this IApplicationBuilder app 
        , ILogger<IEventServiceException> _logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorContext = context.Features.Get<IExceptionHandlerFeature>();
           
                if(errorContext?.Error is IEventServiceException errorData ) 
                {
                    context.Response.StatusCode = (int)errorData.StatusCode;
                    await context.Response.WriteAsync( errorData.ToJson());
                    _logger?.LogError(errorData.EventId, errorContext.Error, errorContext.Error.Message);

                }
                else
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Title = "Something Went wrong",
                        Details = context.TraceIdentifier,
                    }, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
                    _logger?.LogError(new EventId(0,"Unknown"), errorContext.Error, errorContext.Error.Message);
                }
            });
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public static void UseCustomException(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomeExceptionMiddleWare>();
    }


}
