using System;

namespace DynamicSQL.Extencoes
{
    /// <summary>
    /// Classe para definir a tabela do banco de dados nos atributos
    /// </summary>
    public class Table: Attribute
    {
        /// <summary>
        /// Nome da tabela do Banco de Dados
        /// </summary>
        public string Name { get; set; }
    }
}
