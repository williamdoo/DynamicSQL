using DynamicSQL.Libs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    public class Comando:AppSQL
    {
        protected SqlCommand SqlComando { get; set; }
        protected SqlTransaction SqlTran { get; set; }

        protected void BeginTransation()
        {
            SqlTran = SqlComando.Connection.BeginTransaction();
            SqlComando.Transaction = SqlTran;
        }

        protected bool BeginTransationAberto()
        {
            return SqlTran != null;
        }
    }
}
