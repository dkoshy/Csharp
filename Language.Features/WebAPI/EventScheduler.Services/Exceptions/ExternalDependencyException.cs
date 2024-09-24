using System.Net;

namespace EventScheduler.Services.Exceptions;

public class ExternalDependencyException : Exception
{

    public ExternalDependencyException(HttpStatusCode statusCode , object? info = null)
    {
        StatusCode = statusCode;
        Info = info;
    }

    public HttpStatusCode StatusCode { get; }
    public object? Info { get; }
}
