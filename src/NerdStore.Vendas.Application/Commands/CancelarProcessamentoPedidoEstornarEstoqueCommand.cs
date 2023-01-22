using NerdStore.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Commands
{
    public  class CancelarProcessamentoPedidoEstornarEstoqueCommand : Command
    {
        public CancelarProcessamentoPedidoEstornarEstoqueCommand(Guid pedidoId, Guid clienteId)
        {
            AggregateId = pedidoId;
            PedidoId = pedidoId;
            ClienteId = clienteId;
        }

        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }
    }
}
