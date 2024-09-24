using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace EventScheduler.Services.Exceptions;

public interface IEventServiceException
{
    EventId EventId { get; set; }
    HttpStatusCode StatusCode { get; set; }
    int ErrorCode { get; set; }
    string CustomMessage { get; set; }


    string ToJson();
}


public class DefaultEventServiceException : Exception, IEventServiceException
{
    public EventId EventId { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public int ErrorCode { get; set; }
    public string CustomMessage { get; set; } = string.Empty;


    public string ToJson()
    {
        return JsonSerializer.Serialize(new
        {
            this.StatusCode,
            this.ErrorCode,
            this.CustomMessage,
            this.Message,
            this.EventId
        }
       , new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}
