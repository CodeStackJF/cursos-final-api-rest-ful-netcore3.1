using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursos.Models;
namespace Cursos.Controllers
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
        CursosCTX ctx;
        
        public WeatherForecastController(ILogger<WeatherForecastController> logger, CursosCTX _ctx)
        {
            _logger = logger;
            ctx = _ctx;
        }

        [HttpGet]
        public IEnumerable<Estudiante> Get()
        {
           return ctx.Estudiante.ToList();
        }
    }
}
