using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    public static class Conversor
    {
        public static string JuntarParametro(this object parametro, string separado)
        {
            return JuntarParametro(parametro, separado, "");
        }

        public static string JuntarParametro(this object parametro, string separado, string contatena)
        {
            return string.Join(separado, parametro.GetType().GetProperties().Select(t => contatena + t.Name));
        }
    }
}
