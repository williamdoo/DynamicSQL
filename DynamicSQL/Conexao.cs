using DynamicSQL.Flags;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    public class Conexao
    {
        SqlConnection sqlConn;
        public string ConexaoDataBase { get; private set; }
        public int TimeOut { get; set; }
        public System.Data.ConnectionState Estado { get { return sqlConn.State; } }
        public Conexao(string conexao)
        {
            ConexaoDataBase = conexao;
            sqlConn = new SqlConnection(ConexaoDataBase);
        }

        private bool Abrir()
        {
            if (Estado == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
            }
            return Estado == System.Data.ConnectionState.Open;
        }

        private bool Fechar()
        {
            if (Estado == System.Data.ConnectionState.Open)
            {
                sqlConn.Close();
            }
            return Estado == System.Data.ConnectionState.Closed;
        }

        public ComandoSQL AbrirComandoSQL(EnumBegin.Begin beginTrans = EnumBegin.Begin.Nenhum)
        {
            Abrir();
            ComandoSQL com = new ComandoSQL(sqlConn, beginTrans);
            return com;
        }
    }
}
