using DynamicSQL.Extencoes;
using DynamicSQL.Flags;
using DynamicSQL.Libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL
{
    /// <summary>
    /// Classe para realiza as transações de operações no banco de dados
    /// </summary>
    public class ComandoSQL : Comando, IDisposable
    {
        /// <summary>
        /// Obtem ou informa o tempo de espera da tentativa de execução de um comando (tempo em segundos)
        /// </summary>
        public int TimeOut
        {
            get { return SqlComando.CommandTimeout; }
            set { SqlComando.CommandTimeout = value; }
        }

        /// <summary>
        /// Obtem ou informa o tipo de comando a ser executado
        /// </summary>
        public System.Data.CommandType TipoComando
        {
            get { return SqlComando.CommandType; }                                                                  
            set { SqlComando.CommandType = value; }
        }

        /// <summary>
        /// Inicializa um nova instância de transações de operações no banco de dados
        /// </summary>
        /// <param name="con">Define um SqlConnection</param>
        /// <param name="beginTrans">Define o inicio de uma transação</param>
        public ComandoSQL(SqlConnection con, EnumBegin.Begin beginTrans)
        {
            SqlComando = con.CreateCommand();
            SqlComando.Connection = con;
            SqlComando.CommandType = System.Data.CommandType.Text;

            if(beginTrans == EnumBegin.Begin.BeginTransaction)
            {
                BeginTransation();
            }
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <returns>Retorna um IList do tipo da entidade com as informações do bando de dados</returns>
        #region Select
        public IList<T> GetAll<T>() where T : new()
        {
            T t = new T();
            string comandoSelect = "";
            SqlComando.Parameters.Clear();
            comandoSelect = $"SELECT * FROM {GetNomeTabela(t)}";

            return Select<T>(comandoSelect, null);
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <param name="clausulaWhere">Cláusula where para filtrar as informações</param>
        /// <param name="parametros">Parametro com os valores definido para filtrar as informações</param>
        /// <returns>Retorna um IList do tipo da entidade com as informações do bando de dados</returns>
        public IList<T> Get<T>(string clausulaWhere, object parametros) where T : new()
        {
            T t = new T();
            string strSelect = "";

            AddParametro(SqlComando, parametros);
            strSelect = $"SELECT * FROM {GetNomeTabela(t)} WHERE {clausulaWhere.Trim()} ";

            return Select<T>(strSelect, parametros);
        }

        /// <summary>
        /// Obtem uma lista de registro e atribui na entidade
        /// </summary>
        /// <typeparam name="T">Tipo de entidade</typeparam>
        /// <param name="comando">Camando de select</param>
        /// <param name="parametros">Parametro com os valores definido no camando de select</param>
        /// <returns>Retorna um IList do tipo da entidade com as informações do bando de dados</returns>
        public IList<T> Select<T>(string comando, object parametros=null) where T : new()
        {
            List<T> listDynamic = new List<T>();
            AddParametro(SqlComando, parametros);
            SqlComando.CommandText = comando;
            using (SqlDataReader sqldr = SqlComando.ExecuteReader())
            {

                while (sqldr.Read())
                {
                    T t = new T();

                    for (int i = 0; i < sqldr.FieldCount; i++)
                    {
                        SetValorCampo(t, sqldr.GetName(i), sqldr.GetValue(i));
                    }

                    listDynamic.Add(t);
                }
            }
            return listDynamic;
        }

        /// <summary>
        ///  Obtem uma lista de informações e atribui ao Data Set
        /// </summary>
        /// <param name="comando">Camando de select</param>
        /// <returns>Retorna um DataSet as informações do bando de dados</returns>
        public DataSet Select(string comando)
        {
            DataSet ds = new DataSet();

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            SqlDataAdapter sqlda = new SqlDataAdapter(SqlComando);
            sqlda.Fill(ds);
            return ds;
        }
        #endregion

        #region Insert
        /// <summary>
        /// Inserir um registro no bando de dado com as informaçoes de entidade
        /// </summary>
        /// <param name="entidade">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Insert(object entidade)
        {
            int linhaAfetada = 0;
            int? id = null;
            string strInsert = "", nomeCampoIdentity;
            if (entidade != null)
            {
                nomeCampoIdentity = GetNomeIncremento(entidade);
                strInsert = $"INSERT INTO {GetNomeTabela(entidade)} ({entidade.FormatarSintaxe(", ", nomeCampoIdentity)}) values ({entidade.FormatarSintaxe(", ", nomeCampoIdentity, "insert")})";
                AddParametro(SqlComando, entidade, nomeCampoIdentity);

                if (string.IsNullOrWhiteSpace(nomeCampoIdentity))
                {
                    SqlComando.CommandText = strInsert;
                    linhaAfetada = SqlComando.ExecuteNonQuery();
                }
                else
                {
                    strInsert += "; SELECT SCOPE_IDENTITY()";
                    SqlComando.CommandText = strInsert;
                    object obj = SqlComando.ExecuteScalar();

                    if (obj != null)
                    {
                        id = int.Parse(obj.ToString());
                        linhaAfetada = 1;
                        SetValorCampo(entidade, nomeCampoIdentity, id);
                    }
                }
            }

            return linhaAfetada;
        }

        /// <summary>
        /// Inserir um registro no bando de dado onde é informado o comando completo do insert 
        /// </summary>
        /// <param name="entidade">Comando de insert com seus valoes definidos</param>
        /// <returns>Retorna o valor do identity, caso não tenha retorna o valor O</returns>
        public int Insert(string comando)
        {
            comando += "; SELECT SCOPE_IDENTITY()";
            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;

            object obj = SqlComando.ExecuteScalar();

            if (obj != null)
            {
                return int.Parse(obj.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Inserir um registro no bando de dado onde é informado as informações básica do insert 
        /// </summary>
        /// <param name="tabela">Nome da tabela do banco</param>
        /// <param name="parametros">Parâmetro com os valores definido</param>
        /// <returns></returns>
        public int Insert(string tabela, object parametros)
        {
            string strInsert = "";
            AddParametro(SqlComando, parametros);
            strInsert = $"INSERT INTO {tabela} ({parametros.FormatarSintaxe(", ", "")}) values ({parametros.FormatarSintaxe(", ", "", "insert")})";
            return Insert(strInsert);
        }
        #endregion

        #region Update
        /// <summary>
        /// Atualiza o registro do bando de dados com as informaçoes de entidade
        /// </summary>
        /// <param name="entidade">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update(object entidade)
        {
            int linhaAfetada = 0;
            string strUpdate = "", nomeCampoIdentity;
            if (entidade != null)
            {
                nomeCampoIdentity = GetNomeIncremento(entidade);
                AddParametro(SqlComando, entidade);                
                strUpdate = $"UPDATE {GetNomeTabela(entidade)} SET {entidade.FormatarSintaxe(", ", nomeCampoIdentity, "update")} WHERE {nomeCampoIdentity} = @{nomeCampoIdentity}";
                SqlComando.CommandText = strUpdate;

                linhaAfetada = SqlComando.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        /// <summary>
        /// Atualiza os registros no bando de dado onde é informado o comando completo do update 
        /// </summary>
        /// <param name="comando">Comando de update com seus valoes definidos</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update(string comando)
        {
            int linhaAfetada = 0;

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            linhaAfetada = SqlComando.ExecuteNonQuery();

            return linhaAfetada;
        }

        /// <summary>
        /// Atualiza os registros no bando de dado onde é informado as informações básica do update 
        /// </summary>
        /// <param name="tabela">Nome da tabela do banco</param>
        /// <param name="parametros">Parâmetro com os valores definido</param>
        /// <param name="clausulaWhere">Cláusula where para filtrar as informações que serão alteradas</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Update(string tabela, object parametros, string clausulaWhere)
        {
            string strUpdate = "";

            AddParametro(SqlComando, parametros);
            strUpdate = $"UPDATE {tabela} SET {parametros.FormatarSintaxe(", ", "", "update")} WHERE {clausulaWhere}";
            return Update(strUpdate);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Apaga o registro do bando de dados com as informaçoes de entidade
        /// </summary>
        /// <param name="entidade">Objeto entidade da tabela</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Delete(object entidade)
        {            
            int linhaAfetada = 0;
            string strDelete = "", nomeCampoIdentity;

            if (entidade != null)
            {
                nomeCampoIdentity = GetNomeIncremento(entidade);
                AddParametro(SqlComando, entidade);
                strDelete = $"DELETE FROM {GetNomeTabela(entidade)} WHERE {nomeCampoIdentity} = @{nomeCampoIdentity}";
                SqlComando.CommandText = strDelete;

                linhaAfetada = SqlComando.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        /// <summary>
        /// Apaga os registros no bando de dado onde é informado o comando completo do delete 
        /// </summary>
        /// <param name="comando">Comando de delete com seus valoes definido</param>
        /// <returns>Retorna o número de linhas afatadas</returns>
        public int Delete(string comando)
        {
            int linhaAfetada = 0;

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            linhaAfetada = SqlComando.ExecuteNonQuery();

            return linhaAfetada;
        }

        /// <summary>
        /// Apaga os registros no bando de dado onde é informado as informações básica do update 
        /// </summary>
        /// <param name="tabela">Nome da tabela do banco</param>
        /// <param name="clausulaWhere">Cláusula where para filtrar as informações que serão apagadas</param>
        /// <returns></returns>
        public int Delete(string tabela, string clausulaWhere)
        {
            string strDelete = "";
            strDelete = $"DELETE FROM {tabela} WHERE {clausulaWhere}";
            return Delete(strDelete);
        }
        #endregion

        /// <summary>
        /// Libera todos os recursos utilizado na transações
        /// </summary>
        public void Dispose()
        {
            SqlComando.Parameters.Clear();
            SqlComando.Dispose();
            SqlComando.Connection.Close();
            if (SqlTran != null)
            {
                SqlTran.Dispose();
                SqlTran = null;
            }
        }
    }
}
