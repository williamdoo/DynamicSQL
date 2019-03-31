using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    public class MapEntity
    {
        /// <summary>
        /// Classe para definir a tabela do banco de dados nos atributos
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class Table : Attribute
        {
            /// <summary>
            /// Nome da tabela do Banco de Dados
            /// </summary>
            public string Name { get; set; }
        }
        /// <summary>
        /// Classe para definir os atrubutos dos campos da tabela do banco de dados.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class Key : Attribute
        {
            /// <summary>
            /// Chave primária da coluna da tabala do banco
            /// </summary>
            public bool Primarykey { get; private set; } = true;
            /// <summary>
            /// Incremento da coluna da tabala do banco
            /// </summary>
            public bool Increment { get; set; } = false;
        }

        /// <summary>
        /// Classe para definir os atrubutos dos campos da tabela do banco de dados.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class Column : Attribute
        {
            /// <summary>
            /// Nome do campo da tabela
            /// </summary>
            public string Name { get; set; }
        }
    }
}
