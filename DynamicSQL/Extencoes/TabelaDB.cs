using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    /// <summary>
    /// Classe para definir a tabela do banco de dados nos atributos
    /// </summary>
    public class TabelaDB: Attribute
    {
        /// <summary>
        /// Nome da tabela do Banco de Dados
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Retorna o nome da tabela do banco de dados que foi definito no objeto
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="obj">Objeto</param>
        /// <returns>Retorna o nome da tabela</returns>
        public static string GetNomeTabela<T>(T obj)
        {
            TabelaDB[] atributos = (TabelaDB[])typeof(T).GetCustomAttributes(typeof(TabelaDB), true);

            if (atributos.Length > 0)
            {
                return atributos[0].Nome;
            }

            return "";
        }
    }
}
