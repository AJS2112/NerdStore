using MediatR;
using NerdStore.Core.DomainObjects.DTOs;
using NerdStore.Core.Messages.CommonMessages.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.Business.Events
{
    public class PagamentoEventHandler : INotificationHandler<PedidoEstoqueConfirmadoEvent>
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoEventHandler(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        public async Task Handle(PedidoEstoqueConfirmadoEvent notification, CancellationToken cancellationToken)
        {
            var pagamentoPedido = new PagamentoPedido
            {
                PedidoId = notification.PedidoId,
                ClienteId = notification.ClienteId,
                Total = notification.Total,
                NomeCartao = notification.NomeCartao,
                NumeroCartao = notification.NumeroCartao,
                ExpiracaoCartao = notification.ExpiracaoCartao,
                CvvCartao = notification.CvvCartao
            };

            await _pagamentoService.RealizarPagamentoPedido(pagamentoPedido);
        }
    }
}
