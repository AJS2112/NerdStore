using NerdStore.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Events
{
    public class VoucherAplicadoPedidoEvent : Event
    {
        public VoucherAplicadoPedidoEvent(Guid clienteId, Guid peidoId, Guid voucherId)
        {
            ClienteId = clienteId;
            PeidoId = peidoId;
            VoucherId = voucherId;
        }

        public Guid ClienteId { get; private set; }
        public Guid PeidoId { get; private set; }
        public Guid VoucherId { get; private set; }

    }
}
