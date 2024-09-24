using EventScheduler.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventScheduler.Filetrs;
/// <summary>
/// 
/// </summary>
/// 
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ExternalDependencyExceptionFiletrAttribute : Attribute, IActionFilter, IOrderedFilter 
{
    public int Order => int.MaxValue - 10;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuted(ActionExecutedContext context) 
    {
        var exception = context.Exception;
        if(exception is ExternalDependencyException httpresponse)
        {
            context.Result = new ObjectResult(httpresponse.Info)
            {
                StatusCode = (int)httpresponse.StatusCode
            };

            context.ExceptionHandled =  true;
        }
    
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnActionExecuting(ActionExecutingContext context) { }
   
}
