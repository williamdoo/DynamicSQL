using System;

namespace DynamicSQL.Extencoes
{
    /// <summary>
    /// Classe para definir os campos da tabela do banco de dados nos atributos.
    /// </summary>
    public class Column : Attribute
    {
        /// <summary>
        /// Chave primária da coluna da tabala do banco
        /// </summary>
        public bool Primarykey { get; set; } = true;
        /// <summary>
        /// Incremento da coluna da tabala do banco
        /// </summary>
        public bool Increment { get; set; } = false;
    }
}
