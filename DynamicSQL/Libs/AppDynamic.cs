﻿using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Libs
{
    /// <summary>
    /// Aplicações do SQL 
    /// </summary>
    public class AppDynamic
    {
        /// <summary>
        /// Paramentros limpos
        /// </summary>
        protected bool clearParameters = false;
        /// <summary>
        /// Adicionar parâmetros no SQL Command
        /// </summary>
        /// <param name="sqlCommand">Obejto SqlCommand</param>
        /// <param name="parameter">Parâmetro que serão adicionado</param>
        /// <param name="ignoreColumn">Coluna que não será adicionado ao parâmetro</param>
        protected void AddParameter(SqlCommand sqlCommand, object parameter, string ignoreColumn=null)
        {
            sqlCommand.Parameters.Clear();
            clearParameters = true;
            if (parameter != null)
            {
                var teste = parameter.GetType().GetProperties();

                foreach (var item in parameter.GetType().GetProperties().Where(t => (t.CustomAttributes?.FirstOrDefault()?.AttributeType ?? null) != typeof(MapEntity.Ignore)))
                {
                    if (item.Name.ToLower() != (ignoreColumn?.ToLower()??""))
                    {
                        SqlParameter param = new SqlParameter
                        {
                            ParameterName = $"@{item.Name}",
                            Value = (parameter.GetType().GetProperty(item.Name).GetValue(parameter, null) ?? DBNull.Value)
                        };

                        sqlCommand.Parameters.Add(param);
                    }
                }
            }
        }


        /// <summary>
        /// Verifica se o valor que vem do SqlDataReader está vazio
        /// </summary>
        /// <param name="valor">Valo do objeto SqlDataReader</param>
        /// <returns>Retorna true caso o valor está vazio, caso contrário false.</returns>
        private static bool IsDBNull(object valor)
        {
            return valor == Convert.DBNull;
        }

        /// <summary>
        /// Setar o valor em um objeto
        /// </summary>
        /// <param name="obj">Objeto que sera setado o valor</param>
        /// <param name="nameColumn">Nome da prorpiedade do objeto que recebe o valor</param>
        /// <param name="value">Valor que será setado no objeto</param>
        protected void SetValue(object obj, string nameColumn, object value)
        {
            Type type = obj.GetType();
            PropertyInfo propInfo = type.GetProperty(nameColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propInfo != null)
            {
                if (!IsDBNull(value))
                {
                    propInfo.SetValue(obj, value, null);
                }
            }
        }

        /// <summary>
        /// Obtem o nome da coluna de uma entidade definida com o valor true no atributo do tipo incremento
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome do campo da entidade, caso não encontre retorna vazio</returns>
        protected string GetNameIncrement<T>(T obj) where T : MapEntity
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                MapEntity.PrimaryKey[] atributos = (MapEntity.PrimaryKey[])item.GetCustomAttributes(typeof(MapEntity.PrimaryKey), true);

                if (atributos.Length > 0)
                {
                    if (atributos[0].Increment)
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Obtem o nome do campo de uma entidade definida com o valor true no atributo do tipo chave primária
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome do campo da entidade, caso não encontre retorna vazio</returns>
        protected string GetNamePrimaryKey<T>(T obj) where T : MapEntity
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                MapEntity.PrimaryKey[] atributos = (MapEntity.PrimaryKey[])item.GetCustomAttributes(typeof(MapEntity.PrimaryKey), true);

                if (atributos.Length > 0)
                {
                    if (atributos[0].Primarykey)
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Obtem o nome do campo de uma entidade definida com o valor
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome do campo da entidade, caso não encontre retorna o nome padrão do campos definido para entidade</returns>
        protected string GetNameColumn<T>(T obj) where T : MapEntity
        {
            var param = obj.GetType().GetProperties();

            foreach (var item in param)
            {
                MapEntity.Column[] atributos = (MapEntity.Column[])item.GetCustomAttributes(typeof(MapEntity.Column), true);

                if (atributos.Length > 0)
                {
                    if (!string.IsNullOrEmpty(atributos[0].Name))
                    {
                        return item.Name;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Obtem o atributo nome da tabela definido na entidade.
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="obj">Objeto da entidade</param>
        /// <returns>Retorna o nome da tabela, caso não encontre retonra vazio</returns>
        public static string GetNameTable<T>(T obj) where T: MapEntity
        {
            MapEntity.Table[] atributos = (MapEntity.Table[])obj.GetType().GetCustomAttributes(typeof(MapEntity.Table), true);

            if (atributos.Length > 0)
            {
                return atributos[0].Name;
            }

            return "";
        }
    }
}
