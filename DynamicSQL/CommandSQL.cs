using DynamicSQL.Extencoes;
using DynamicSQL.Flags;
using DynamicSQL.Libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DynamicSQL
{
    /// <summary>
    /// Classe para realiza as transações de operações no banco de dados
    /// </summary>
    public class CommandSQL : Command, IDisposable
    {
        /// <summary>
        /// Obtem ou informa o tempo de espera da tentativa de execução de um comando (tempo em segundos)
        /// </summary>
        public int TimeOut
        {
            get { return SqlCommand.CommandTimeout; }
            set { SqlCommand.CommandTimeout = value; }
        }

        /// <summary>
        /// Obtem ou informa o tipo de comando a ser executado
        /// </summary>
        public System.Data.CommandType CommandType
        {
            get { return SqlCommand.CommandType; }                                                                  
            set { SqlCommand.CommandType = value; }
        }

        /// <summary>
        /// Inicializa um nova instância de transações de operações no banco de dados
        /// </summary>
        /// <param name="con">Define um SqlConnection</param>
        /// <param name="beginTrans">Define o inicio de uma transação</param>
        public CommandSQL(SqlConnection con, EnumBegin.Begin beginTrans)
        {
            SqlCommand = con.CreateCommand();
            SqlCommand.Connection = con;
            SqlCommand.CommandType = System.Data.CommandType.Text;

            if(beginTrans == EnumBegin.Begin.BeginTransaction)
            {
                BeginTransation();
            }
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <returns>Retorna um IList do tipo da entidade com as informações do banco de dados</returns>
        #region Select
        public IList<T> GetAll<T>() where T : MapEntity, new()
        {
            T t = new T();
            string commSelect = "";
            SqlCommand.Parameters.Clear();
            commSelect = $"SELECT {t.FormatSintaxe(", ", "")} FROM {GetNameTable(t)}";

            return Select<T>(commSelect, null);
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidades        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <param name="clauseWhere">Cláusula where para filtrar as informações</param>
        /// <param name="parameters">Parametro com os valores definido para filtrar as informações</param>
        /// <returns>Retorna um IList do tipo da entidade com as informações do banco de dados</returns>
        public IList<T> Get<T>(string clauseWhere, object parameters) where T : MapEntity, new()
        {
            T t = new T();
            string strSelect;
            AddParameter(SqlCommand, parameters);
            strSelect = $"SELECT {t.FormatSintaxe(", ", "")} FROM {GetNameTable(t)} WHERE {clauseWhere.Trim()} ";

            return Select<T>(strSelect, parameters);
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <param name="command">Camando de select</param>
        /// <param name="parameters">Parametro com os valores definido no camando de select</param>
        /// <returns>Retorna um IList do tipo da entidade com as informações do banco de dados</returns>
        public IList<T> Select<T>(string command, object parameters=null) where T : new()
        {
            List<T> listDynamic = new List<T>();
            AddParameter(SqlCommand, parameters);
            SqlCommand.CommandType = System.Data.CommandType.Text;
            SqlCommand.CommandText = command;
            using (SqlDataReader sqldr = SqlCommand.ExecuteReader())
            {

                while (sqldr.Read())
                {
                    T t = new T();

                    for (int i = 0; i < sqldr.FieldCount; i++)
                    {
                        SetValue(t, sqldr.GetName(i), sqldr.GetValue(i));
                    }

                    listDynamic.Add(t);
                }
            }
            return listDynamic;
        }

        /// <summary>
        ///  Obtem uma lista de informações e atribui ao Data Set
        /// </summary>
        /// <param name="command">Camando de select</param>
        /// <param name="parameters">Parametro com os valores definido no camando de select</param>
        /// <returns>Retorna um DataSet as informações do banco de dados</returns>
        public DataSet Select(string command, object parameters = null)
        {
            DataSet ds = new DataSet();
            AddParameter(SqlCommand, parameters);
            SqlCommand.Parameters.Clear();
            SqlCommand.CommandText = command;
            SqlDataAdapter sqlda = new SqlDataAdapter(SqlCommand);
            sqlda.Fill(ds);
            return ds;
        }
        #endregion

        #region Insert
        /// <summary>
        /// Inserir um registro no banco de dado com as informaçoes de entidade
        /// </summary>
        /// <param name="entity">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Insert<T>(T entity) where T: MapEntity
        {
            int linhaAfetada = 0;
            int? id = null;
            string strInsert = "", nomeCampoIdentity;
            if (entity != null)
            {
                nomeCampoIdentity = GetNameIncrement(entity);
                strInsert = $"INSERT INTO {GetNameTable(entity)} ({entity.FormatSintaxe(", ", nomeCampoIdentity)}) values ({entity.FormatSintaxe(", ", nomeCampoIdentity, "insert")})";
                AddParameter(SqlCommand, entity, nomeCampoIdentity);
                SqlCommand.CommandType = System.Data.CommandType.Text;
                if (string.IsNullOrWhiteSpace(nomeCampoIdentity))
                {                    
                    SqlCommand.CommandText = strInsert;
                    linhaAfetada = SqlCommand.ExecuteNonQuery();
                }
                else
                {
                    strInsert += "; SELECT SCOPE_IDENTITY()";
                    SqlCommand.CommandText = strInsert;
                    object obj = SqlCommand.ExecuteScalar();

                    if (obj != null)
                    {
                        id = int.Parse(obj.ToString());
                        linhaAfetada = 1;
                        SetValue(entity, nomeCampoIdentity, id);
                    }
                }
            }

            return linhaAfetada;
        }



        /// <summary>
        /// Inserir um registro no banco de dado onde é informado o comando completo do insert 
        /// </summary>
        /// <param name="command">Comando de insert com seus valoes definidos</param>
        /// <returns>Retorna o valor do identity, caso não tenha retorna o valor O</returns>
        public int Insert(string command)
        {
            command += "; SELECT SCOPE_IDENTITY()";
            if (!clearParameters)
            {
                SqlCommand.Parameters.Clear();
            }
            SqlCommand.CommandType = System.Data.CommandType.Text;
            SqlCommand.CommandText = command;

            object obj = SqlCommand.ExecuteScalar();

            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                return int.Parse(obj.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Inserir um registro no banco de dado onde é informado as informações básica do insert 
        /// </summary>
        /// <param name="table">Nome da tabela do banco</param>
        /// <param name="parameters">Parâmetro com os valores definido</param>
        /// <returns></returns>
        public int Insert(string table, object parameters)
        {
            string strInsert = "";
            AddParameter(SqlCommand, parameters);
            strInsert = $"INSERT INTO {table} ({parameters.FormatSintaxe(", ", "")}) values ({parameters.FormatSintaxe(", ", "", "insert")})";
            return Insert(strInsert);
        }
        #endregion

        #region Update
        /// <summary>
        /// Atualiza o registro do banco de dados com as informaçoes de entidade
        /// </summary>
        /// <param name="entity">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update<T>(T entity) where T: MapEntity
        {
            int linhaAfetada = 0;
            string strUpdate = "", nomeCampoChave;
            if (entity != null)
            {
                nomeCampoChave = GetNamePrimaryKey(entity);
                AddParameter(SqlCommand, entity);                
                strUpdate = $"UPDATE {GetNameTable(entity)} SET {entity.FormatSintaxe(", ", nomeCampoChave, "update")} WHERE {nomeCampoChave} = @{nomeCampoChave}";
                SqlCommand.CommandType = System.Data.CommandType.Text;
                SqlCommand.CommandText = strUpdate;

                linhaAfetada = SqlCommand.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        /// <summary>
        /// Atualiza os registros no banco de dado onde é informado o comando completo do update 
        /// </summary>
        /// <param name="command">Comando de update com seus valoes definidos</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update(string command)
        {
            int linhaAfetada = 0;

            if (!clearParameters)
            {
                SqlCommand.Parameters.Clear();
            }
            SqlCommand.CommandType = System.Data.CommandType.Text;
            SqlCommand.CommandText = command;
            linhaAfetada = SqlCommand.ExecuteNonQuery();

            return linhaAfetada;
        }

        /// <summary>
        /// Atualiza os registros no banco de dado onde é informado as informações básica do update 
        /// </summary>
        /// <param name="table">Nome da tabela do banco</param>
        /// <param name="parameters">Parâmetro com os valores definido</param>
        /// <param name="clauseWhere">Cláusula where para filtrar as informações que serão alteradas</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update(string table, object parameters, string clauseWhere)
        {
            string strUpdate = "";
            AddParameter(SqlCommand, parameters);
            strUpdate = $"UPDATE {table} SET {parameters.FormatSintaxe(", ", "", "update")} WHERE {clauseWhere}";
            return Update(strUpdate);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Apaga o registro do banco de dados com as informaçoes de entidade
        /// </summary>
        /// <param name="entity">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Delete<T>(T entity) where T : MapEntity
        {            
            int linhaAfetada = 0;
            string strDelete = "", nomeCampoChave;

            if (entity != null)
            {
                nomeCampoChave = GetNamePrimaryKey(entity);
                AddParameter(SqlCommand, entity);
                strDelete = $"DELETE FROM {GetNameTable(entity)} WHERE {nomeCampoChave} = @{nomeCampoChave}";
                SqlCommand.CommandType = System.Data.CommandType.Text;
                SqlCommand.CommandText = strDelete;

                linhaAfetada = SqlCommand.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        /// <summary>
        /// Apaga os registros no banco de dado onde é informado o comando completo do delete 
        /// </summarycommand
        /// <param name="command">Comando de delete com seus valoes definido</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Delete(string command)
        {
            int linhaAfetada = 0;

            SqlCommand.Parameters.Clear();
            SqlCommand.CommandType = System.Data.CommandType.Text;
            SqlCommand.CommandText = command;
            linhaAfetada = SqlCommand.ExecuteNonQuery();

            return linhaAfetada;
        }

        /// <summary>
        /// Apaga os registros no banco de dado onde é informado as informações básica do update 
        /// </summary>
        /// <param name="table">Nome da tabela do banco</param>
        /// <param name="clauseWhere">Cláusula where para filtrar as informações que serão apagadas</param>
        /// <returns></returns>
        public int Delete(string table, string clauseWhere)
        {
            string strDelete = "";
            strDelete = $"DELETE FROM {table} WHERE {clauseWhere}";
            return Delete(strDelete);
        }
        #endregion

        #region Procedure
        /// <summary>
        /// Executar Stored Procedure e atribui ao Data Set
        /// </summary>
        /// <param name="nameProcedure">Nome da precedure</param>
        /// <param name="parameters">Parâmetro com os valores definido</param>
        /// <returns>Retorna um DataSet as informações da Stored Procedure</returns>
        public DataSet ExecuteStoredProcedure(string nameProcedure, object parameters=null)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter sqlda;
            SqlCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                AddParameter(SqlCommand, parameters);
            }

            SqlCommand.CommandText = nameProcedure;
            sqlda = new SqlDataAdapter(SqlCommand);
            sqlda.Fill(ds);

            return ds;
        }

        /// <summary>
        /// Executar Stored Procedure e atribui a uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <param name="nameProcedure">Nome da precedure</param>
        /// <param name="parameters">Parâmetro com os valores definido</param>
        /// <returns>Retorna um IList do tipo da entidade com as informações da Stored Procedure</returns>
        public IList<T> ExecuteStoredProcedure<T>(string nameProcedure, object parameters = null) where T:new()
        {
            List<T> listDynamic = new List<T>();

            SqlCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                AddParameter(SqlCommand, parameters);
            }

            SqlCommand.CommandText = nameProcedure;

            using (SqlDataReader sqldr = SqlCommand.ExecuteReader())
            {
                while (sqldr.Read())
                {
                    T t = new T();

                    for (int i = 0; i < sqldr.FieldCount; i++)
                    {
                        SetValue(t, sqldr.GetName(i), sqldr.GetValue(i));
                    }

                    listDynamic.Add(t);
                }
            }
            return listDynamic;
        }
        #endregion

        /// <summary>
        /// Libera todos os recursos utilizado na transações
        /// </summary>
        public void Dispose()
        {
            SqlCommand.Parameters.Clear();
            SqlCommand.Dispose();
            SqlCommand.Connection.Close();
            if (SqlTran != null)
            {
                SqlTran.Dispose();
                SqlTran = null;
            }
        }
    }
}
