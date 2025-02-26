using knowledge_sharing_platform_cloud.config;
using knowledge_sharing_platform_cloud.Entity;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ApplicationDBContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ApplicationDBContext context)
        {
            _logger = logger;
            _dbContext = context;   
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "CreateUser")]
        public Boolean post()
        {
            User user = new User();
            user.Email = "emaknk";
            user.Profile_Url = "ndkjaks";
            user.Username = "fbjfj";
            user.Password = "knkjnjka";
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
