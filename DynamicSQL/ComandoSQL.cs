using DynamicSQL.Extencoes;
using DynamicSQL.Flags;
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
            SqlComando = new SqlCommand();
            SqlComando.Connection = con;
            SqlComando.CommandType = System.Data.CommandType.Text;

            if(beginTrans == EnumBegin.Begin.BeginTransaction)
            {
                BeginTransation();
            }
        }

        private void BeginTransation()
        {
            SqlTran = SqlComando.Connection.BeginTransaction();
        }

        public void Commit()
        {
            SqlTran.Commit();
        }

        public void Rollback()
        {
            SqlTran.Rollback();
        }

        private void AddParametro(object parametro)
        {
            foreach (var item in parametro.GetType().GetProperties())
            {
                SqlComando.Parameters.Add(new SqlParameter($"@{item.Name}", parametro.GetType().GetProperty(item.Name).GetValue(parametro, null)));
            }
        }
       
        #region Select
        public IList<T> GetTodos<T>() where T : new()
        {
            T t = new T();
            string comandoSelect = "";

            TabelaDB[] atributos = (TabelaDB[])typeof(T).GetCustomAttributes(typeof(TabelaDB), true);
            if (atributos.Length > 0)
            {
                TabelaDB atributo = atributos[0];
                comandoSelect = $"SELECT * FROM {atributo.Nome}";
            }
            return Consultar<T>(comandoSelect);
        }

        public IList<T> Get<T>(string clausulaWhere, object parametros) where T : new()
        {
            T t = new T();
            string comandoSelect = "";

            AddParametro(parametros);
            TabelaDB[] atributos = (TabelaDB[])typeof(T).GetCustomAttributes(typeof(TabelaDB), true);
            if (atributos.Length > 0)
            {
                TabelaDB atributo = atributos[0];
                comandoSelect = $"SELECT * FROM {atributo.Nome} {clausulaWhere.Trim()} ";
            }
            return Consultar<T>(comandoSelect);
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

        public IList<T> Consultar<T>(string comando) where T : new()
        {
            List<T> listDynamic = new List<T>();

            SqlComando.CommandText = comando;
            SqlDataReader sqldr = SqlComando.ExecuteReader();
            
            while (sqldr.Read())
            {
                T t = new T();

                for (int i = 0; i < sqldr.FieldCount; i++)
                {
                    Type type = t.GetType();
                    PropertyInfo prop = type.GetProperty(sqldr.GetName(i));

                    if (prop != null)
                    {
                        if (!DBNull(sqldr.GetValue(i), prop.PropertyType))
                        {
                            prop.SetValue(t, sqldr.GetValue(i), null);
                        }
                    }
                }

                listDynamic.Add(t);
            }

            sqldr.Close();
            return listDynamic;
        }
        #endregion

        public int Inserir(string comando, object entidade=null)
        {
            if (entidade != null)
            {

            }

            SqlComando.CommandText = comando;
            return SqlComando.ExecuteNonQuery();
        }

        public int Update(string Comando)
        {
            SqlComando.CommandText = Comando;
            return SqlComando.ExecuteNonQuery();
        }

        public int Deletar(string comando)
        {
            SqlComando.CommandText = comando;
            return SqlComando.ExecuteNonQuery();
        }

        private bool DBNull(object value, object defaultValue)
        {
            return value == Convert.DBNull;
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
