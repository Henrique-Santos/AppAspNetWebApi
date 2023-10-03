using Api.Controllers;
using Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteVersionController : MainController
    {
        private readonly ILogger<TesteVersionController> _logger;
        public TesteVersionController(INotificador notificador, IUser user, ILogger<TesteVersionController> logger) : base(notificador, user) 
        {
            _logger = logger;
        }

        [HttpGet]
        public string Valor()
        {            
            _logger.LogTrace("Trace message");
            _logger.LogDebug("Debug message");
            _logger.LogInformation("Info message");
            _logger.LogWarning("Warning message");
            _logger.LogError("Error message");
            _logger.LogCritical("Critical message");
            _logger.LogError(new NullReferenceException(), "An exception");            
            return "Sou a V2";
        }
    }
}