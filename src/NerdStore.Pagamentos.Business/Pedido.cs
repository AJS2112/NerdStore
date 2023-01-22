using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.Business
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
