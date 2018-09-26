using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Flags
{
    public class EnumBegin
    {
        [Flags]
        public enum Begin
        {
            Nenhum=0,
            BeginTransaction = 1
        }
    }
}
