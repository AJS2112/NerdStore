using MediatR;
using Microsoft.AspNetCore.Mvc;
using NerdStore.Core.Commnunication.Mediator;
using NerdStore.Core.Messages.CommonMessages.Notifications;

namespace NerdStore.WebApp.MVC.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatrHandler _mediatrHandler;

        protected ControllerBase(INotificationHandler<DomainNotification> notifications, IMediatrHandler mediatrHandler)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediatrHandler = mediatrHandler;
        }

        protected Guid ClienteId = Guid.Parse("d4cfbde5-7db2-4b13-b80f-a6a229651afb");

        protected bool OperacaoValida()
        {
            return !_notifications.TemNotificacao();
        }

        protected IEnumerable<string> ObterMensagensErro()
        {
            return _notifications.ObterNotificacoes().Select(c => c.Value).ToList();
        }

        protected void NotificarErro(string codigo, string mensagem)
        {
            _mediatrHandler.PublicarNotificacao(new DomainNotification(codigo, mensagem));
        }
    }
}
