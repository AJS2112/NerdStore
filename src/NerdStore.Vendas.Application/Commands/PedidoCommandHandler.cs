using MediatR;
using NerdStore.Core.Commnunication.Mediator;
using NerdStore.Core.DomainObjects.DTOs;
using NerdStore.Core.Extensions;
using NerdStore.Core.Messages;
using NerdStore.Core.Messages.CommonMessages.DomainEvents;
using NerdStore.Core.Messages.CommonMessages.IntegrationEvents;
using NerdStore.Core.Messages.CommonMessages.Notifications;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : 
        IRequestHandler<AdicionarItemPedidoCommand, bool>,
        IRequestHandler<AtualizarItemPedidoCommand, bool>,
        IRequestHandler<RemoverItemPedidoCommand, bool>,
        IRequestHandler<AplicarVoucherPedidoCommand, bool>,
        IRequestHandler<IniciarPedidoCommand, bool>,
        IRequestHandler<FinalizarPedidoCommand, bool>,
        IRequestHandler<CancelarProcessamentoPedidoEstornarEstoqueCommand, bool>,
        IRequestHandler<CancelarProcessamentoPedidoCommand, bool>


    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediatrHandler _mediatrHandler;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediatrHandler mediatrHandler)
        {
            _pedidoRepository = pedidoRepository;
            _mediatrHandler = mediatrHandler;
        }
    
        public async Task<bool> Handle(AdicionarItemPedidoCommand command, CancellationToken cancellationToken)
        {
            if (ValidarComando(command)) return false;
            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(command.ClienteId);
            var pedidoItem = new PedidoItem(command.ProdutoId, command.Nome, command.Quantidade, command.ValorUnitario);

            if(pedido == null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(command.ClienteId);
                pedido.AdicionarItem(pedidoItem);

                _pedidoRepository.Adicionar(pedido);
                pedido.AdicionarEvento(new PedidoRascunhoIniciadoEvent(command.ClienteId, pedido.Id));
            }
            else
            {
                var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);
                pedido.AdicionarItem(pedidoItem);

                if (pedidoItemExistente)
                {
                    _pedidoRepository.AtualizarItem(pedido.PedidoItens.FirstOrDefault(i => i.ProdutoId == pedidoItem.Id));
                }
                else
                {
                    _pedidoRepository.AtualizarItem(pedidoItem);
                }

                pedido.AdicionarEvento(new PedidoAtualizadoEvent(pedido.ClienteId, pedido.Id, pedido.ValorTotal));
            }

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(pedido.ClienteId, pedido.Id, command.ProdutoId, command.Nome, command.ValorUnitario, command.Quantidade));

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarItemPedidoCommand command, CancellationToken cancellationToken)
        {
            if (!ValidarComando(command)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(command.ClienteId);

            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, command.ProdutoId);
            if (!pedido.PedidoItemExistente(pedidoItem))
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Item do pedido não encontrado"));
                return false;
            }

            pedido.AtualizarUnidade(pedidoItem, command.Quantidade);
            pedido.AdicionarEvento(new PedidoProdutoAtualizadoEvent(pedido.ClienteId, pedido.Id, command.ProdutoId, command.Quantidade));

            _pedidoRepository.AtualizarItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(RemoverItemPedidoCommand command, CancellationToken cancellationToken)
        {
            if (!ValidarComando(command)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(command.ClienteId);

            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, command.ProdutoId);
            if (!pedido.PedidoItemExistente(pedidoItem))
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Item do pedido não encontrado"));
                return false;
            }

            pedido.RemoverItem(pedidoItem);
            pedido.AdicionarEvento(new PedidoProdutoRemovidoEvent(pedido.ClienteId, pedido.Id, command.ProdutoId));

            _pedidoRepository.RemoverItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AplicarVoucherPedidoCommand command, CancellationToken cancellationToken)
        {
            if (!ValidarComando(command)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(command.ClienteId);
            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            var voucher = await _pedidoRepository.ObterVoucherPorCodigo(command.CodigoVoucher);
            if (voucher == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Voucher não encontrado"));
                return false;
            }

            var voucherAplicacaoValidation = pedido.AplicarVoucher(voucher);
            if (!voucherAplicacaoValidation.IsValid)
            {
                foreach (var error in voucherAplicacaoValidation.Errors)
                {
                    await _mediatrHandler.PublicarNotificacao(new DomainNotification(error.ErrorCode, error.ErrorMessage));
                }

                return false;
            }

            pedido.AdicionarEvento(new PedidoAtualizadoEvent(pedido.ClienteId, pedido.Id, pedido.ValorTotal));
            pedido.AdicionarEvento(new VoucherAplicadoPedidoEvent(pedido.ClienteId, pedido.Id, voucher.Id));

            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(IniciarPedidoCommand command, CancellationToken cancellationToken)
        {
            if (!ValidarComando(command)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(command.ClienteId);
            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            pedido.IniciarPedido();

            var itensList = new List<Item>();
            pedido.PedidoItens.ForEach(i => itensList.Add(new Item { Id = i.ProdutoId, Quantidade = i.Quantidade }));
            var listaProdutosPedido = new ListaProdutosPedido { PedidoId = pedido.Id, Itens = itensList };

            pedido.AdicionarEvento(new PedidoIniciadoEvent(pedido.Id, pedido.ClienteId, pedido.ValorTotal, listaProdutosPedido,  command.NomeCartao, command.NumeroCartao, command.NumeroCartao, command.CvvCartao));

            _pedidoRepository.Atualizar(pedido);
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(FinalizarPedidoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorId(command.PedidoId);
            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            pedido.FinalizarPedido();

            pedido.AdicionarEvento(new PedidoFinalizadoEvent(command.PedidoId));
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(CancelarProcessamentoPedidoEstornarEstoqueCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorId(command.PedidoId);
            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            var itensList = new List<Item>();
            pedido.PedidoItens.ForEach(i => itensList.Add(new Item { Id = i.Id, Quantidade = i.Quantidade }));
            var listaProdutosPedido = new ListaProdutosPedido { PedidoId = pedido.Id, Itens = itensList };

            pedido.AdicionarEvento(new PedidoProcessamentoCanceladoEvent(command.PedidoId, pedido.ClienteId, listaProdutosPedido));
            pedido.TornarRascunho();
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(CancelarProcessamentoPedidoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorId(command.PedidoId);
            if (pedido == null)
            {
                await _mediatrHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado"));
                return false;
            }

            pedido.TornarRascunho();
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        private bool ValidarComando(Command request)
        {
            if (request.EhValido()) return true;

            foreach (var error in request.ValidationResult.Errors)
            {
                _mediatrHandler.PublicarNotificacao(new DomainNotification(request.MessageType, error.ErrorMessage));
            }

            return false;
        }
    }
}
