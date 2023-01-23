using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class EventStoreService : IEventStoreService
    {
        private readonly IEventStoreConnection _connection;

        public EventStoreService(IConfiguration configuration )
        {
            _connection = EventStoreConnection.Create(configuration.GetConnectionString("EventStoreConnection"));
            _connection.ConnectAsync();
        }
        public IEventStoreConnection GetConnection()
        {
            return _connection;
        }
    }
}
