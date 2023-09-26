using Api.Controllers;
using Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteVersionController : MainController
    {
        public TesteVersionController(INotificador notificador, IUser user) : base(notificador, user) { }

        [HttpGet]
        public string Valor()
        {
            return "Sou a V1";
        }
    }
}