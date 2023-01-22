using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.AntiCorruption
{
    public interface IPayPalGateway
    {
        string GetPayPalServiceKey(string apiKey, string encriptionKey);
        string GetCardHashKey(string serviceKey, string cartaoCredito);
        bool CommitTransaction(string cardHashKey, string orderId, decimal amount);
    }
}
