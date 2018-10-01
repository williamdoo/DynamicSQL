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
    public class ComandoSQL : Comando, IDisposable
    {
        public int TimeOut
        {
            get { return SqlComando.CommandTimeout; }
            set { SqlComando.CommandTimeout = value; }
        }

        public System.Data.CommandType TipoComando
        {
            get { return SqlComando.CommandType; }                                                                  
            set { SqlComando.CommandType = value; }
        }

        public ComandoSQL(SqlConnection con, EnumBegin.Begin beginTrans)
        {
            SqlComando = con.CreateCommand();
            SqlComando.Connection = con;
            SqlComando.CommandType = System.Data.CommandType.Text;

            if(beginTrans == EnumBegin.Begin.BeginTransaction)
            {
                SqlTran = con.BeginTransaction();
                SqlComando.Transaction = SqlTran;
            }
        }


        public void Commit()
        {
            SqlTran.Commit();
        }

        public void Rollback()
        {
            SqlTran.Rollback();
        }

        #region Select
        public IList<T> GetAll<T>() where T : new()
        {
            T t = new T();
            string comandoSelect = "";
            SqlComando.Parameters.Clear();
            comandoSelect = $"SELECT * FROM {GetNomeTabela(t)}";

            return Select<T>(comandoSelect, null);
        }

        public IList<T> Get<T>(string clausulaWhere, object parametros) where T : new()
        {
            T t = new T();
            string strSelect = "";

            AddParametro(SqlComando, parametros);
            strSelect = $"SELECT * FROM {GetNomeTabela(t)} WHERE {clausulaWhere.Trim()} ";

            return Select<T>(strSelect, parametros);
        }

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

        public DataSet Select(string comando)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            SqlDataAdapter sqlda = new SqlDataAdapter(SqlComando);
            sqlda.Fill(ds);
            return ds;
        }
        #endregion

        #region Insert
        public int Insert(object entidade)
        {
            int linhaAfetada = 0;
            int? id = null;
            string strInsert = "", nomeCampoIdentity;
            if (entidade != null)
            {
                nomeCampoIdentity = GetNomeIncremento(entidade);
                strInsert = $"INSERT INTO {GetNomeTabela(entidade)} ({entidade.ConcatenarCampos(", ", nomeCampoIdentity)}) values ({entidade.ConcatenarCampos(", ", nomeCampoIdentity, "insert")})";
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

        public int Insert(string tabela, object parametros)
        {
            string strInsert = "";
            AddParametro(SqlComando, parametros);
            strInsert = $"INSERT INTO {tabela} ({parametros.ConcatenarCampos(", ", "")}) values ({parametros.ConcatenarCampos(", ", "", "insert")})";
            return Insert(strInsert);
        }
        #endregion

        #region Update
        public int Update(object entidade)
        {
            int linhaAfetada = 0;
            string strUpdate = "", nomeCampoChave;
            if (entidade != null)
            {
                nomeCampoChave = GetNomeChavePrimaria(entidade);
                AddParametro(SqlComando, entidade);                
                strUpdate = $"UPDATE {GetNomeTabela(entidade)} SET {entidade.ConcatenarCampos(", ", nomeCampoChave, "update")} WHERE {nomeCampoChave} = @{nomeCampoChave}";
                SqlComando.CommandText = strUpdate;

                linhaAfetada = SqlComando.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        public int Update(string comando)
        {
            int linhaAfetada = 0;

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            linhaAfetada = SqlComando.ExecuteNonQuery();

            return linhaAfetada;
        }

        public int Update(string tabela, object parametros, string clausulaWhere)
        {
            string strUpdate = "";

            AddParametro(SqlComando, parametros);
            strUpdate = $"UPDATE {tabela} SET {parametros.ConcatenarCampos(", ", "", "update")} WHERE {clausulaWhere}";
            return Update(strUpdate);
        }
        #endregion

        #region Delete
        public int Delete(object entidade)
        {            
            int linhaAfetada = 0;
            string strDelete = "", nomeCampoChave;

            if (entidade != null)
            {
                nomeCampoChave = GetNomeChavePrimaria(entidade);
                AddParametro(SqlComando, entidade);
                strDelete = $"DELETE FROM {GetNomeTabela(entidade)} WHERE {nomeCampoChave} = @{nomeCampoChave}";
                SqlComando.CommandText = strDelete;

                linhaAfetada = SqlComando.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        public int Delete(string comando)
        {
            int linhaAfetada = 0;

            SqlComando.Parameters.Clear();
            SqlComando.CommandText = comando;
            linhaAfetada = SqlComando.ExecuteNonQuery();

            return linhaAfetada;
        }

        public int Delete(string tabela, string clausulaWhere)
        {
            string strDelete = "";
            strDelete = $"DELETE FROM {tabela} WHERE {clausulaWhere}";
            return Delete(strDelete);
        }
        #endregion

        public void Dispose()
        {
            SqlComando.Parameters.Clear();
            SqlComando.Dispose();
            SqlComando.Connection.Close();
            if (SqlTran != null)
            {
                SqlTran.Dispose();
            }
        }
    }
}
