using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    public class AppSQL
    {
        protected void AddParametro(SqlCommand sqlComando, object parametro, string ignoreCampo=null)
        {
            sqlComando.Parameters.Clear();
            if (parametro != null)
            {
                foreach (var item in parametro.GetType().GetProperties())
                {
                    if (item.Name != ignoreCampo)
                    {
                        SqlParameter param = new SqlParameter();

                        param.ParameterName = $"@{item.Name}";
                        param.Value = (parametro.GetType().GetProperty(item.Name).GetValue(parametro, null)?? DBNull.Value);

                        sqlComando.Parameters.Add(param);
                    }
                }
            }
        }


        private static bool EDBNull(object value, object defaultValue)
        {
            return value == Convert.DBNull;
        }

        protected void SetValorCampo(object obj, string nomeCampo, object valor)
        {
            Type type = obj.GetType();
            PropertyInfo propInfo = type.GetProperty(nomeCampo);

            if (propInfo != null)
            {
                if (!EDBNull(valor, propInfo.PropertyType))
                {
                    propInfo.SetValue(obj, valor, null);
                }
            }
        }

        protected string GetNomeIncremento<T>(T obj)
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                CampoDB[] atributos = (CampoDB[])item.GetCustomAttributes(typeof(CampoDB), true);

                if (atributos.Length > 0)
                {
                    if (atributos[0].Incremento)
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        protected string GetNomeChavePrimaria<T>(T obj)
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                CampoDB[] atributos = (CampoDB[])item.GetCustomAttributes(typeof(CampoDB), true);

                if (atributos.Length > 0)
                {
                    if (atributos[0].ChavePrimaria)
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Retorna o nome da tabela do banco de dados que foi definito no objeto
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="obj">Objeto</param>
        /// <returns>Retorna o nome da tabela</returns>
        public static string GetNomeTabela<T>(T obj)
        {
            TabelaDB[] atributos = (TabelaDB[])obj.GetType().GetCustomAttributes(typeof(TabelaDB), true);

            if (atributos.Length > 0)
            {
                return atributos[0].Nome;
            }

            return "";
        }
    }
}
