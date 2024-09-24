using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace EventScheduler;
/// <summary>
/// 
/// </summary>
public class AppProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _behaviouroptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public AppProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
    {
        _behaviouroptions = options.Value;
    }
    /// <inheritdoc/>
    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        if (_behaviouroptions.ClientErrorMapping.TryGetValue(statusCode ?? 0, out var clientErrorMapping))
        {
            problemDetails.Title ??= clientErrorMapping.Title;
            problemDetails.Type ??= clientErrorMapping.Link;
        }

        problemDetails.Extensions.Add("myvalue2", "value2");
        problemDetails.Extensions.Add("myvalue3", "value3");

        var traceId = httpContext?.TraceIdentifier ?? string.Empty;
        problemDetails.Extensions.Add("traceId", traceId);

        //problemDetails.Extensions["traceId"] = traceId;

        return problemDetails;

    }


    /// <inheritdoc/>

    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        var validationDetails = new ValidationProblemDetails()
        {
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        return validationDetails;
    }
}
