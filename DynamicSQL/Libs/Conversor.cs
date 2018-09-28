using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    public static class Conversor
    {
        public static string ConcatenarCampos(this object parametro, string separado)
        {
            return ConcatenarCampos(parametro, separado, "" , "");
        }

        public static string ConcatenarCampos(this object parametro, string separado, string ignoreCampo)
        {
            return ConcatenarCampos(parametro, separado, ignoreCampo, "");
        }

        public static string ConcatenarCampos(this object parametro, string separado, string ignoreCampo, string tipoSitaxe)
        {
            switch (tipoSitaxe)
            {
                case "insert":
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => "@" + t.Name));
                case "update":
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => t.Name+ " = @" + t.Name));
                default:
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => t.Name));
            }
        }
    }
}
