using NerdStore.Core.Commnunication.Mediator;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.Data
{
    public static class MediatrExtension
    {
        public static async Task PublicarEventos(this IMediatrHandler mediatrHandler, PagamentoContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            domainEntities
                .ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            var tasks = domainEvents.Select(async (domainEvent) => {
                await mediatrHandler.PublicarEvento(domainEvent);
            });

            await Task.WhenAll(tasks);
        }
    }
}
