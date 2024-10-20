using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DecoratorDesignPattern.Models;
using DecoratorDesignPattern.WeatherInterface;

namespace DecoratorDesignPattern.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IWeatherService _weatherService;

        public HomeController(ILoggerFactory loggerFactory
          , IWeatherService weatherService) //   , IConfiguration configuration ,, IMemoryCache memoryCache

        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<HomeController>();


            _weatherService = weatherService;
            /*
             String apiKey = configuration.GetValue<String>("AppSettings:OpenWeatherMapApiKey");
            var coreWeatherService = new WeatherService(apiKey);
            var weatherServiceWihLogger = new WeatherServiceLogger(coreWeatherService, _loggerFactory.CreateLogger<WeatherServiceLogger>());
            var weatherServiceWihcache = new WeatherServiceCache(weatherServiceWihLogger, memoryCache);
            _weatherService = weatherServiceWihcache;*/
        }


        public IActionResult Index(string location = "Chicago")
        {
            CurrentWeather conditions = _weatherService.GetCurrentWeather(location);
            return View(conditions);
        }



        public IActionResult Forecast(string location = "Chicago")
        {
            LocationForecast forecast = _weatherService.GetForecast(location);
            return View(forecast);
        }

        public IActionResult ApiKey()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
