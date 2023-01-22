using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.AntiCorruption
{
    public interface IConfigurationManager
    {
        string GetValue(string node);
    }
}
