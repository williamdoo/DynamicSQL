using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    public class Comando
    {
        protected SqlCommand SqlComando { get; set; }
        protected SqlTransaction SqlTran { get; set; }
    }
}
