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
    public class Connection
    {
        /// <summary>
        /// SqlConnection
        /// </summary>
        SqlConnection sqlConn;
        /// <summary>
        /// Cadeia de informações para realizar a abertura de uma conexão com o banco de dados
        /// </summary>
        public static string ConnectionDataBase { get; set; }
        /// <summary>
        /// Obtem o tempo de espera da tentativa de abertura da conexão com o banco de dado (tempo em segundos)
        /// </summary>
        public int TimeOut { get { return sqlConn.ConnectionTimeout; } }
        /// <summary>
        /// Indicação do estado da conexão com o banco de dados
        /// </summary>
        public System.Data.ConnectionState State { get { return sqlConn.State; } }

        /// <summary>
        /// Iniciar uma nova instância com uma cadeia de informações para abrir uma conexão com o banco de dados
        /// </summary>
        public Connection()
        {
            sqlConn = new SqlConnection(ConnectionDataBase);
        }

        /// <summary>
        /// Iniciar uma nova instância com uma cadeia de informações para abrir uma conexão com o banco de dados
        /// </summary>
        /// <param name="connectionString"></param>
        public Connection(string connectionString)
        {
            ConnectionDataBase = connectionString;
            sqlConn = new SqlConnection(ConnectionDataBase);
        }

        /// <summary>
        /// Abrir a conexão com o banco de dados
        /// </summary>
        /// <returns></returns>
        private bool Open()
        {
            if (State == System.Data.ConnectionState.Closed)
            {
                sqlConn.Open();
            }
            return State == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (State == System.Data.ConnectionState.Open)
            {
                sqlConn.Close();
            }
            return State == System.Data.ConnectionState.Closed;
        }

        /// <summary>
        /// Abrir uma nova transação de operações do banco de dados
        /// </summary>
        /// <param name="beginTrans">Abrir um ponto inicial de uma tranação</param>
        /// <param name="timeOut">Informa o tempo de espera da tentativa de execução de um comando (tempo em segundos) padrão 30 segundo</param>
        /// <returns>Retorna o ComandoSQL para realizar as operações no banco de dados.</returns>
        public CommandSQL OpenCommandSQL(EnumBegin.Begin beginTrans = EnumBegin.Begin.None, int timeOut=30)
        {
            Open();
            CommandSQL com = new CommandSQL(sqlConn, beginTrans);
            com.TimeOut = timeOut;
            return com;
        }
    }
}
