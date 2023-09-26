using Business.Intefaces;
using Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;
        public readonly IUser User;

        protected Guid UsuarioID { get; set; }
        protected bool UsuarioAutenticado { get; set; }

        protected MainController(INotificador notificador, IUser user)
        {
            _notificador = notificador;
            User = user;

            if (user.IsAuthenticated())
            {
                UsuarioID = user.GetUserId();
                UsuarioAutenticado = true;
            }
        }


        protected ActionResult RespostaCustomizada(object result = null)
        {
            if (!OperacaoValida())
            {
                return BadRequest(new
                {
                    success = false,
                    errors = _notificador.ObterNotificacoes().Select(x => x.Mensagem)
                });
            }
            return Ok(new
            {
                success = true,
                data = result
            });
        }

        protected ActionResult RespostaCustomizada(ModelStateDictionary modelState)
        {
            if (!ModelState.IsValid) NotificarErroModelInvalida(modelState);
            return RespostaCustomizada();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(x => x.Errors);
            foreach (var erro in erros)
            {
                var mensagem = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(mensagem);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }
    }
}