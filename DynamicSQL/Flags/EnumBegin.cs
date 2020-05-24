using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Flags
{
    /// <summary>
    /// Clase de enumeração de begin
    /// </summary>
    public class EnumBegin
    {
        /// <summary>
        /// Enumeração do bengin transaction
        /// </summary>
        [Flags]
        public enum Begin
        {
            /// <summary>
            /// Nenhum transação aberta atual
            /// </summary>
            None = 0,
            /// <summary>
            /// Abertura de uma transação atual
            /// </summary>
            BeginTransaction = 1
        }
    }
}
