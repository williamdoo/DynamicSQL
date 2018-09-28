using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    public class AppSQL
    {
        protected void AddParametro(SqlCommand sqlComando, object parametro)
        {
            if (parametro != null)
            {
                foreach (var item in parametro.GetType().GetProperties())
                {
                    sqlComando.Parameters.Add(new SqlParameter($"@{item.Name}", parametro.GetType().GetProperty(item.Name).GetValue(parametro, null)));
                }
            }
        }
    }
}
