using EventStore.ClientAPI;
using NerdStore.Core.Data.EventSourcing;
using NerdStore.Core.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class EventSourcingRepository : IEventSourcingRepository
    { 
        private readonly IEventStoreService _eventStoreService;

        public EventSourcingRepository(IEventStoreService eventStoreService)
        {
            _eventStoreService = eventStoreService;
        }

        public Task<IEnumerable<StoredEvent>> ObterEventos(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public async Task SalvarEvento<TEvent>(TEvent evento) where TEvent : Event
        {
            await _eventStoreService
                .GetConnection()
                .AppendToStreamAsync(
                    evento.AggregateId.ToString(),
                    ExpectedVersion.Any,
                    FormatarEvento(evento)
                    );
        }

        public async Task<IEnumerable<StoredEvent>> ObterEventps(Guid aggregateId)
        {
            var eventos = await _eventStoreService
                .GetConnection()
                .ReadStreamEventsBackwardAsync(aggregateId.ToString(), 0, 500, false);
            
            var listaEventos = new List<StoredEvent>();
            foreach (var item in eventos.Events)
            {
                var dataEncoded = Encoding.UTF8.GetString(item.Event.Data);
                var jsonData = JsonConvert.DeserializeObject<Event>(dataEncoded);

                var evento = new StoredEvent(
                    item.Event.EventId, 
                    item.Event.EventType, 
                    jsonData.Timestamp, 
                    dataEncoded);
            }

            return listaEventos;
        }

        private static IEnumerable<EventData> FormatarEvento<TEvent>(TEvent evento) where TEvent : Event
        {
            yield return new EventData(
                Guid.NewGuid(),
                evento.MessageType,
                true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evento)),
                null);
        }
    }
}
