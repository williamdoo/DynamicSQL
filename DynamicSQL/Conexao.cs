using DynamicSQL.Flags;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    /// <summary>
    /// Classe para abrir uma conexão com o banco de dados
    /// </summary>
    public class Conexao
    {
        /// <summary>
        /// SqlConnection
        /// </summary>
        SqlConnection sqlConn;
        /// <summary>
        /// Cadeia de informações para realizar a abertura de uma conexão com o banco de dados
        /// </summary>
        public string ConexaoDataBase { get; private set; }
        /// <summary>
        /// Obtem o tempo de espera da tentativa de abertura da conexão com o banco de dado (tempo em segundos)
        /// </summary>
        public int TimeOut { get { return sqlConn.ConnectionTimeout; } }
        /// <summary>
        /// Indicação do estado da conexão com o bando de dados
        /// </summary>
        public System.Data.ConnectionState Estado { get { return sqlConn.State; } }
        /// <summary>
        /// Inicia uma nova instância com uma cadeia de informações para abrir uma conexão com o bando de dados
        /// </summary>
        /// <param name="conexao"></param>
        public Conexao(string conexao)
        {
            ConexaoDataBase = conexao;
            sqlConn = new SqlConnection(ConexaoDataBase);
        }

        /// <summary>
        /// Abrir a conexão com o bando de dados
        /// </summary>
        /// <returns></returns>
        private bool Abrir()
        {
            if (Estado == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
            }
            return Estado == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados
        /// </summary>
        /// <returns></returns>
        public bool Fechar()
        {
            if (Estado == System.Data.ConnectionState.Open)
            {
                sqlConn.Close();
            }
            return Estado == System.Data.ConnectionState.Closed;
        }

        /// <summary>
        /// Abrir uma nova transação de operações do bando de dados
        /// </summary>
        /// <param name="beginTrans">Abrir um ponto inicial de uma tranação</param>
        /// <returns>Retorna o ComandoSQL para realizar as operações no bando de dados.</returns>
        public ComandoSQL AbrirComandoSQL(EnumBegin.Begin beginTrans = EnumBegin.Begin.Nenhum)
        {
            Abrir();
            ComandoSQL com = new ComandoSQL(sqlConn, beginTrans);
            return com;
        }
    }
}
