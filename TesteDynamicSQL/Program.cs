﻿using DynamicSQL;
using DynamicSQL.Libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteDynamicSQL.Entidade;

namespace TesteDynamicSQL
{
    class Program
    {
        private static Connection conexao;
        static void Main(string[] args)
        {
            conexao = new Connection("Server=NomeServidor;Database=BancoDados;User Id=Usuario;Password=Senha;");
            CarregarDadosClasseTipada();
            Console.ReadKey();
        }

        private static void CarregarDadosDataSet()
        {
            Stopwatch sw = Stopwatch.StartNew();
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                DataSet dt = comando.Select("select * from Clientes");
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta DataSet - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarDadosClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Cliente> clientes;
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                clientes = comando.Select<Cliente>("select * from Clientes").ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarTodosDadosClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Cliente> cliente;
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                cliente = comando.GetAll<Cliente>().ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarDadosEspecificoClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Cliente> cliente;
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                cliente = comando.Get<Cliente>("NOME_CLI like @NOME_CLI", new { NOME_CLI = "Vania" }).ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void InserirCliente()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                Cliente cliente = new Cliente()
                {
                    NOME_CLI = "Vania",
                    CPF_CLI = "12345678900",
                    DATANASC_CLI = DateTime.Parse("1992-01-14"),
                    TELEFONE_CLI ="11987654321"
                };

                int id = cliente.ID_CLI;
                int linhas = comando.Insert(cliente);
            }
        }

        private static void InserirClienteBegin()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                try
                {
                    Cliente cliente = new Cliente()
                    {
                        NOME_CLI = "Vania",
                        CPF_CLI = "12345678900",
                        DATANASC_CLI = DateTime.Parse("1992-01-14"),
                        TELEFONE_CLI = "11987654321"
                    };

                    comando.Insert(cliente);

                    Cliente cliente2 = new Cliente()
                    {
                        NOME_CLI = "William",
                        CPF_CLI = "14785236999",
                        DATANASC_CLI = DateTime.Parse("1988-05-17"),
                        TELEFONE_CLI = "119321456987"
                    };

                    comando.Insert(cliente2);

                    comando.Commit();
                }
                catch
                {
                    comando.Rollback();
                }
            }
        }


        private static void InserirClienteComando()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                int id = comando.Insert("CLIENTES", new { NOME_CLI = "Vania", CPF_CLI = "12345678900", DATANASC_CLI = "1992-01-14", TELEFONE_CLI = "11987654321" });
            }
        }


        private static void UpdateCliente()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                int linhasAlterada = comando.Update("update CLIENTES set TELEFONE_CLI = '119258741369' where ID_CLI = 123");
            }
        }

        private static void UpdateClienteBegin()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comando.Update("update CLIENTES set TELEFONE_CLI = '119258741369' where ID_CLI = 123");
                comando.Commit();
            }
        }

        private static void UpdateCliente2()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comando.Update("CLIENTES", new { TELEFONE_CLI = "119258741369" }, "ID_CLI = 123");
                comando.Commit();
            }
        }

        private static void UpdateClienteTipado()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                Cliente fone = comando.Get<Cliente>("ID_CLI = @ID_CLI", new { ID_CLI = 123 }).FirstOrDefault();

                fone.TELEFONE_CLI = "119258741369";
                int linhasAlterada = comando.Update(fone);
                comando.Commit();
            }
        }

        private static void DeleteCliente()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL())
            {
                int linhasAlterada = comando.Delete("delete from CLIENTES where ID_CLI = 123");
            }
        }

        private static void DeleteClienteBegin()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comando.Delete("delete from CLIENTES where ID_CLI = 123");
                comando.Commit();
            }
        }

        private static void DeleteCliente2()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comando.Delete("CLIENTES", "ID_CLI = 123");
                comando.Commit();
            }
        }

        private static void DeleteClienteTipado()
        {
            using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                Cliente fone = comando.Get<Cliente>("ID_CLI = @ID_CLI", new { ID_FONE = 123 }).FirstOrDefault();

                int linhasAlterada = comando.Delete(fone);
                comando.Commit();
            }
        }
    }
}