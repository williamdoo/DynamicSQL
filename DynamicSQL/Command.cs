using DynamicSQL.Libs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    /// <summary>
    /// Classe que representa as instruções o T-SQL
    /// </summary>
    public class Command:AppDynamic
    {
        /// <summary>
        /// Instrução do T-SQL ou amarzenamento de comando SQL no bando de dados
        /// </summary>
        protected SqlCommand SqlCommand { get; set; }
        /// <summary>
        /// Trasação realizado no bando de dados
        /// </summary>
        protected SqlTransaction SqlTran { get; set; }

        /// <summary>
        /// Inicia uma transação no banco de dados
        /// </summary>
        protected void BeginTransation()
        {
            SqlTran = SqlCommand.Connection.BeginTransaction();
            SqlCommand.Transaction = SqlTran;
        }

        /// <summary>
        /// Confirma a transação realizada no bando de dados
        /// </summary>
        public void Commit()
        {
            SqlTran.Commit();
        }

        /// <summary>
        /// Retroceder uma transação que foi feita no bando de dados e que está no estado pendente
        /// </summary>
        public void Rollback()
        {
            SqlTran.Rollback();
        }
    }
}
