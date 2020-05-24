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
                    var qInsert = from c in parameter.GetType().GetProperties()
                                  where c.Name != ignoreColumn &&
                                        (c.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(Ignore)
                                  select new
                                  {
                                      Name = "@" + c.Name
                                  };
                    return string.Join(separate, qInsert.Select(t => "@" + t.Name));
                case "update":
                    var qUpdate = from c in parameter.GetType().GetProperties()
                                where c.Name != ignoreColumn &&
                                      (c.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(Ignore)
                                select new
                                {
                                    Name = " = @"+c.Name
                                };
                    return string.Join(separate, qUpdate.Select(t=>t.Name));
                case "select":
                    var qSelect = from c in parameter.GetType().GetProperties()
                                where c.Name != ignoreColumn &&
                                      (c.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(Ignore)
                                select new
                                {
                                    c.Name
                                };

                    return string.Join(separate, qSelect.Select(t=>t.Name));
            }

            return "";
        }
    }
}
