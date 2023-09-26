using Api.Controllers;
using Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteVersionController : MainController
    {
        public TesteVersionController(INotificador notificador, IUser user) : base(notificador, user) { }

        [HttpGet]
        public string Valor()
        {
            return "Sou a V2";
        }
    }
}