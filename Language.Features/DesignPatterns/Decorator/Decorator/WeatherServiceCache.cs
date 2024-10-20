using DecoratorDesignPattern.WeatherInterface;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DecoratorDesignPattern.Decorator;

public class WeatherServiceCache : IWeatherService
{
    private readonly IWeatherService _innerweatherservice;
    private readonly IMemoryCache _cache;

    public WeatherServiceCache(IWeatherService innerweatherservice,
        IMemoryCache cache)
    {
        _innerweatherservice = innerweatherservice;
        _cache = cache;
    }
    public CurrentWeather GetCurrentWeather(string location)
    {
        var cachekey = $"cahchecurrentcityWeather::{location}";
        if (_cache.TryGetValue<CurrentWeather>(cachekey, out var weather))
        {
            return weather;
        }
        else
        {
            var currentweather = _innerweatherservice.GetCurrentWeather(location);
            _cache.Set(cachekey, currentweather, TimeSpan.FromMinutes(120));
            return currentweather;

        }

    }

    public LocationForecast GetForecast(string location)
    {
        var cachekey =  $"forcatedweatherforcity::{location}";
        if(_cache.TryGetValue<LocationForecast>(cachekey, out var locationForecast))
        {
            return locationForecast;
        }
        else
        {
            var forcast = _innerweatherservice.GetForecast(location);
            _cache.Set(cachekey, forcast, TimeSpan.FromMinutes(120));
            return forcast;
        }

    }
}
