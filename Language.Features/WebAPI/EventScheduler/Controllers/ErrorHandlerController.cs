using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduler.Controllers;

/// <summary>
/// Error handler - global.
/// </summary>
///
[ApiExplorerSettings(IgnoreApi =true)]
[Route("[controller]")]
[ApiController]
[AllowAnonymous]

public class ErrorHandlerController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="env"></param>
    /// <returns></returns>
    /// 
    [Route("error-development")]
    public IActionResult ErrorOnDevelopment([FromServices] IHostEnvironment env)
    {
        if(!env.IsDevelopment())
            return NotFound();
        var exceptionHandlerFrature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return Problem(detail: exceptionHandlerFrature?.Error.StackTrace,
            title: exceptionHandlerFrature?.Error.Message);
    }

    /// <summary>
    /// global error handler endpoint.
    /// </summary>
    /// <returns></returns>
    /// 
    [Route("error")]
    public IActionResult Error() => Problem();
}
