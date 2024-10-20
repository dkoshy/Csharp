using DecoratorDesignPattern.WeatherInterface;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DecoratorDesignPattern.Decorator;

public class WeatherServiceLogger : IWeatherService
{
    private readonly IWeatherService _innerService;
    private readonly ILogger<WeatherServiceLogger> _logger;

    public WeatherServiceLogger(IWeatherService innerService, ILogger<WeatherServiceLogger>
        logger)
    {
        _innerService = innerService;
        _logger = logger;
    }

    public CurrentWeather GetCurrentWeather(string location)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var currentWeather = _innerService.GetCurrentWeather(location);
        stopWatch.Stop();
        return currentWeather;
    }

    public LocationForecast GetForecast(string location)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var forcast = _innerService.GetForecast(location);
        stopWatch.Stop();
        return forcast;

    }
}
