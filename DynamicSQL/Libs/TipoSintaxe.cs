using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    /// <summary>
    /// Tipos de sintaxe que e gerado nos camandos SQL
    /// </summary>
    public static class TipoSintaxe
    {
        /// <summary>
        /// Formata a sintaxe utilizada em camando SQL
        /// </summary>
        /// <param name="parametro">Campos que vão ser formatado para sintaxe do SQL</param>
        /// <param name="separado">Separador dos campos</param>
        /// <param name="ignoreCampo">Campo ignorado na sintaxe</param>
        /// <returns>Retorna os campos entruturados que se enquadra na sintaxe do SQL</returns>
        public static string FormatarSintaxe(this object parametro, string separado, string ignoreCampo)
        {
            return FormatarSintaxe(parametro, separado, ignoreCampo, "select");
        }

        /// <summary>
        /// Formata a sintaxe utilizada em camando SQL
        /// </summary>
        /// <param name="parametro">Campos que vão ser formatado para sintaxe do SQL</param>
        /// <param name="separado">Separador dos campos</param>
        /// <param name="ignoreCampo">Campo ignorado na sintaxe</param>
        /// <param name="tipoSitaxe">Tipo de sintaxe que pode ser montado
        /// <para>tipos: select, insert e update</para>
        /// </param>
        /// <returns>Retorna os campos entruturados que se enquadra na sintaxe do SQL</returns>
        public static string FormatarSintaxe(this object parametro, string separado, string ignoreCampo, string tipoSitaxe)
        {
            switch (tipoSitaxe)
            {
                case "insert":
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => "@" + t.Name));
                case "update":
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => t.Name+ " = @" + t.Name));
                case "select":
                    return string.Join(separado, parametro.GetType().GetProperties().Where(t => t.Name != ignoreCampo).Select(t => t.Name));
            }

            return "";
        }
    }
}
