using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Core.Messages.CommonMessages.IntegrationEvents
{
    public class PagamentoRealizadoEvent : IntegrationEvent
    {
        public PagamentoRealizadoEvent(Guid pedidoId, Guid clienteId, Guid pagamentoId, Guid transacaoId, decimal total)
        {
            AggregateId = pedidoId; 
            PedidoId = pedidoId;
            ClienteId = clienteId;
            PagamentoId = pagamentoId;
            TransacaoId = transacaoId;
            Total = total;
        }

        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }
        public Guid PagamentoId { get; private set; }
        public Guid TransacaoId { get; private set; }
        public decimal Total { get; private set; }
    }
}
