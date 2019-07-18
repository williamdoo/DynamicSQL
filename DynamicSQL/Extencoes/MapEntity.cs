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
            /// lasse para definir a tabela do banco de dados nos atributos
            /// </summary>
            /// <param name="name">Nome da tabela do Banco de Dados</param>
            public Table(string name)
            {
                Name = name;
            }
            /// <summary>
            /// Nome da tabela do Banco de Dados
            /// </summary>
            public string Name { get; private set; }
        }
        /// <summary>
        /// Classe para definir os atrubutos dos campos da tabela do banco de dados.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class PrimaryKey : Attribute
        {
            /// <summary>
            /// Chave primária da coluna da tabala do banco
            /// </summary>
            public bool Primarykey { get; private set; } = true;
            /// <summary>
            /// Incremento da coluna da tabala do banco
            /// </summary>
            public bool Increment { get; set; } = false;

            ///// <summary>
            ///// Classe para definir os atrubutos dos campos da tabela do banco de dados.
            ///// </summary>
            ///// <param name="increment">Incremento da coluna da tabala do banco</param>
            //public PrimaryKey(bool increment=false)
            //{
            //    Increment = increment;
            //}
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
            public string Name { get; private set; }

            /// <summary>
            /// Classe para definir os atrubutos dos campos da tabela do banco de dados.
            /// </summary>
            /// <param name="name">Nome do campo da tabela</param>
            public Column(string name)
            {
                Name = name;
            }
        }
    }
}
