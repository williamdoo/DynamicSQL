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
        public IList<T> GetTodos<T>() where T : new()
        {
            T t = new T();
            string comandoSelect = "";

            comandoSelect = $"SELECT * FROM {GetNomeTabela(t)}";

            return Consultar<T>(comandoSelect, null);
        }

        public IList<T> Get<T>(string clausulaWhere, object parametros) where T : new()
        {
            T t = new T();
            string comandoSelect = "";

            AddParametro(SqlComando, parametros);
            comandoSelect = $"SELECT * FROM {GetNomeTabela(t)} {clausulaWhere.Trim()} ";

            return Consultar<T>(comandoSelect, parametros);
        }

        public IList<T> Consultar<T>(string comando, object parametros=null) where T : new()
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

        public DataSet Consultar(string comando)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            SqlComando.CommandText = comando;
            SqlDataAdapter sqlda = new SqlDataAdapter(SqlComando);
            sqlda.Fill(ds);
            return ds;
        }
        #endregion

        #region Insert
        public int Inserir(object entidade)
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

        public int Inserir(string comando)
        {
            comando += "; SELECT SCOPE_IDENTITY()";
            SqlComando.CommandText = comando;

            object obj = SqlComando.ExecuteScalar();

            if (obj != null)
            {
                return int.Parse(obj.ToString());
            }

            return 0;
        }

        public int Inserir(string tabela, object parametros)
        {
            string strInsert = "";
            AddParametro(SqlComando, parametros);
            strInsert = $"INSERT INTO {tabela} ({parametros.ConcatenarCampos(", ", "")}) values ({parametros.ConcatenarCampos(", ", "", "insert")})";
            return Inserir(strInsert);
        }
        #endregion

        #region Update
        public int Alterar(object entidade)
        {
            int linhaAfetada = 0;
            string strUpdate = "", nomeCampoIdentity;

            if (entidade != null)
            {
                nomeCampoIdentity = GetNomeIncremento(entidade);
                AddParametro(SqlComando, entidade);                
                strUpdate = $"UPDATE {GetNomeTabela(entidade)} SET {entidade.ConcatenarCampos(", ", nomeCampoIdentity, "update")} WHERE {nomeCampoIdentity} = @{nomeCampoIdentity}";
                SqlComando.CommandText = strUpdate;

                linhaAfetada = SqlComando.ExecuteNonQuery();
            }

            return linhaAfetada;
        }

        public int Alterar(string comando)
        {
            int linhaAfetada = 0;

            SqlComando.CommandText = comando;
            linhaAfetada = SqlComando.ExecuteNonQuery();

            return linhaAfetada;
        }

        public int Alterar(string tabela, object parametros, string clausulaWhere)
        {
            string strUpdate = "";
            AddParametro(SqlComando, parametros);
            strUpdate = $"UPDATE {tabela} SET {parametros.ConcatenarCampos(", ", "", "update")} {clausulaWhere}";
            return Alterar(strUpdate);
        }

        #endregion
        public int Apagar(string comando)
        {
            SqlComando.CommandText = comando;
            return SqlComando.ExecuteNonQuery();
        }

        public void Dispose()
        {
            SqlComando.Dispose();
            SqlComando.Connection.Close();
            if (SqlTran != null)
            {
                SqlTran.Dispose();
            }
        }
    }
}
