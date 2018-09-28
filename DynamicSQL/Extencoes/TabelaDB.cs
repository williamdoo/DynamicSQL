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
    }
}
