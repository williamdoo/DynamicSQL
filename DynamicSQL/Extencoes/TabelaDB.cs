using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    public class TabelaDB: Attribute
    {
        public string Nome { get; set; }
    }
}
