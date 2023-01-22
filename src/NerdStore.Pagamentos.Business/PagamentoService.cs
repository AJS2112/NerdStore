using NerdStore.Core.Commnunication.Mediator;
using NerdStore.Core.DomainObjects.DTOs;
using NerdStore.Core.Messages.CommonMessages.IntegrationEvents;
using NerdStore.Core.Messages.CommonMessages.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.Business
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoCartaoCreditoFacade _pagamentoCartaoCreditoFacade;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMediatrHandler _mediatrHandler;

        public PagamentoService(IPagamentoCartaoCreditoFacade pagamentoCartaoCreditoFacade, IPagamentoRepository pagamentoRepository, IMediatrHandler mediatrHandler)
        {
            _pagamentoCartaoCreditoFacade = pagamentoCartaoCreditoFacade;
            _pagamentoRepository = pagamentoRepository;
            _mediatrHandler = mediatrHandler;
        }

        public async Task<Transacao> RealizarPagamentoPedido(PagamentoPedido pagamentoPedido)
        {
            var pedido = new Pedido
            {
                Id = pagamentoPedido.PedidoId,
                Valor = pagamentoPedido.Total
            };

            var pagamento = new Pagamento
            {
                Valor = pagamentoPedido.Total,
                NomeCartao = pagamentoPedido.NomeCartao,
                NumeroCartao = pagamentoPedido.NumeroCartao,
                ExpiracaoCartao = pagamentoPedido.ExpiracaoCartao,
                CvvCartao = pagamentoPedido.CvvCartao,
                PedidoId = pagamentoPedido.PedidoId
            };

            var transacao = _pagamentoCartaoCreditoFacade.RealizarPagamento(pedido, pagamento);

            if (transacao.StatusTransacao == StatusTransacao.Pago)
            {
                pagamento.AdicionarEvento(new PagamentoRealizadoEvent(pedido.Id, pagamentoPedido.ClienteId, pagamento.Id, transacao.Id, pagamento.Valor));

                _pagamentoRepository.Adicionar(pagamento);
                _pagamentoRepository.AdicionarTransacao(transacao);

                await _pagamentoRepository.UnitOfWork.Commit();
                return transacao;
            }

            await _mediatrHandler.PublicarNotificacao(new DomainNotification("pagamento", "A operadora recusou o pagamento"));
            await _mediatrHandler.PublicarEvento(new PagamentoRecusadoEvent(pedido.Id, pagamentoPedido.ClienteId, pagamento.Id, transacao.Id, pagamento.Valor));

            return transacao;
        }
    }
}
