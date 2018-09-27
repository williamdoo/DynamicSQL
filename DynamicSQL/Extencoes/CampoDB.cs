using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    /// <summary>
    /// Classe para definir os campos da tabela do banco de dados nos atributos.
    /// </summary>
    public class CampoDB : Attribute
    {
        /// <summary>
        /// Chave primária do campo da tabala do banco
        /// </summary>
        public bool ChavePrimaria { get; set; } = true;
        /// <summary>
        /// Incremento do campo da tabala do banco
        /// </summary>
        public bool Incremento { get; set; } = true;
    }
}
