using System.Text.Json;

namespace EventScheduler.Helpers;

/// <summary>
/// 
/// </summary>
public static class Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public  static string GetJson(this object obj)
    {
       return JsonSerializer.Serialize(obj
           , new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}
