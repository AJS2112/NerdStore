using MediatR;
using NerdStore.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        public async Task<bool> Handle(AdicionarItemPedidoCommand request, CancellationToken cancellationToken)
        {
            if (ValidarComando(request)) return false;

            return true;
        }

        private bool ValidarComando(Command request)
        {
            if (request.EhValido()) return true;

            foreach (var error in request.ValidationResult.Errors)
            {
                // lançar evento de erro
            }

            return false;
        }
    }
}
