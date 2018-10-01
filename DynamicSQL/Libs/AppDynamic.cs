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
    /// <summary>
    /// Aplicações do SQL 
    /// </summary>
    public class AppDynamic
    {
        /// <summary>
        /// Adicionar parâmetros no SQL Command
        /// </summary>
        /// <param name="sqlComando">Obejto SqlCommand</param>
        /// <param name="parametro">Parâmetro que serão adicionado</param>
        /// <param name="ignoreCampo">Campo que não será adicionado ao parÂmetro</param>
        protected void AddParametro(SqlCommand sqlComando, object parametro, string ignoreCampo=null)
        {
            sqlComando.Parameters.Clear();
            if (parametro != null)
            {
                foreach (var item in parametro.GetType().GetProperties())
                {
                    if (item.Name != ignoreCampo)
                    {
                        sqlComando.Parameters.Add(new SqlParameter($"@{item.Name}", parametro.GetType().GetProperty(item.Name).GetValue(parametro, null)));
                    }
                }
            }
        }


        /// <summary>
        /// Verifica se o valor que vem do SqlDataReader está vazio
        /// </summary>
        /// <param name="valor">Valo do objeto SqlDataReader</param>
        /// <returns>Retorna true caso o valor está vazio, caso contrário false.</returns>
        private static bool DBNull(object valor)
        {
            return valor == Convert.DBNull;
        }

        /// <summary>
        /// Setar o valor em um objeto
        /// </summary>
        /// <param name="obj">Objeto que sera setado o valor</param>
        /// <param name="nomeCampo">Nome da prorpiedade do objeto que recebe o valor</param>
        /// <param name="valor">Valor que será setado no objeto</param>
        protected void SetValorCampo(object obj, string nomeCampo, object valor)
        {
            Type type = obj.GetType();
            PropertyInfo propInfo = type.GetProperty(nomeCampo);

            if (propInfo != null)
            {
                if (!DBNull(valor))
                {
                    propInfo.SetValue(obj, valor, null);
                }
            }
        }

        /// <summary>
        /// Obtem o nome do campo de uma entidade definida com o atributo do tipo chave primário e incremento
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome do campo da entidade, caso não encontre retorna vazio</returns>
        protected string GetNomeIncremento<T>(T obj)
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                CampoDB[] atributos = (CampoDB[])item.GetCustomAttributes(typeof(CampoDB), true);

                if (atributos.Length > 0)
                {
                    if (atributos[0].ChavePrimaria && atributos[0].Incremento)
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Obtem o atributo nome da tabela definido na entidade.
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome da tabela, caso não encontre retonra vazio</returns>
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
