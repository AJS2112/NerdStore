using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Core.Data.EventSourcing
{
    public class StoredEvent
    {
        public StoredEvent(Guid guid, string tipo, DateTime dataOcorrencia, string dados)
        {
            Guid = guid;
            Tipo = tipo;
            DataOcorrencia = dataOcorrencia;
            Dados = dados;
        }

        public Guid Guid { get; private set; }
        public string Tipo { get; private set; }
        public DateTime DataOcorrencia { get; private set; }
        public string Dados { get; private set; }


    }
}
