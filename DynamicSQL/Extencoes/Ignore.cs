using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    /// <summary>
    /// Ignorar o campo nos comando de Select, Insert e Updade quando não forem explícito no camando
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore:Attribute
    {

    }
}
