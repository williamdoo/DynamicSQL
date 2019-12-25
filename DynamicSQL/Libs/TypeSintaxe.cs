using DynamicSQL.Extencoes;
using System.Linq;

namespace DynamicSQL.Libs
{
    /// <summary>
    /// Tipos de sintaxe que e gerado nos camandos SQL
    /// </summary>
    public static class TypeSintaxe
    {
        /// <summary>
        /// Formatar a sintaxe utilizada em comando SQL
        /// </summary>
        /// <param name="parameter">Campos que vão ser formatado para sintaxe do SQL</param>
        /// <param name="separate">Separador dos campos</param>
        /// <param name="ignoreColumn">Campo ignorado na sintaxe</param>
        /// <returns>Retorna os campos entruturados que se enquadra na sintaxe do SQL</returns>
        public static string FormatSintaxe(this object parameter, string separate, string ignoreColumn)
        {
            return FormatSintaxe(parameter, separate, ignoreColumn, "select");
        }

        /// <summary>
        /// Formatar a sintaxe utilizada em camando SQL
        /// </summary>
        /// <param name="parameter">Campos que vão ser formatado para sintaxe do SQL</param>
        /// <param name="separate">Separador dos campos</param>
        /// <param name="ignoreColumn">Campo ignorado na sintaxe</param>
        /// <param name="typeSitaxe">Tipo de sintaxe que pode ser montado
        /// <para>tipos: select, insert e update</para>
        /// </param>
        /// <returns>Retorna os campos entruturados que se enquadra na sintaxe do SQL</returns>
        public static string FormatSintaxe(this object parameter, string separate, string ignoreColumn, string typeSitaxe)
        {
            switch (typeSitaxe)
            {
                case "insert":
                    return string.Join(separate, parameter.GetType().GetProperties().Where(t => t.Name != ignoreColumn && (t.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(MapEntity.Ignore)).Select(t => "@" + t.Name));
                case "update":
                    return string.Join(separate, parameter.GetType().GetProperties().Where(t => t.Name != ignoreColumn && (t.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(MapEntity.Ignore)).Select(t => t.Name + " = @" + t.Name));
                case "insertcolumn":
                    return string.Join(separate, parameter.GetType().GetProperties().Where(t => t.Name != ignoreColumn && (t.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(MapEntity.Ignore)).Select(t => t.Name));
                case "select":
                    return string.Join(separate, parameter.GetType().GetProperties().Where(t => t.CanWrite == true && t.Name != ignoreColumn && (t.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(MapEntity.Ignore)).Select(t => t.Name));
            }

            return "";
        }
    }
}
