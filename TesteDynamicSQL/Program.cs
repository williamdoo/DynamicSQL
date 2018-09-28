using DynamicSQL;
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
        private static Conexao con;
        static void Main(string[] args)
        {
            con = new Conexao("");
            //CarregarDadosDataSet();
            //CarregarDadosClasseTipada();
            //CarregarTodosDadosClasseTipada();
            //CarregarDadosEspecificoClasseTipada();
            //InserirTelefoneBegin();
            //InserirTelefoneBegin();
            //UpdateTelefone();
            //UpdateTelefoneBegin();
            //UpdateTelefone2();
            //UpdateTelefoneTipado();
            //DeleteTelefone();
            //DeleteTelefoneBegin();
            //DeleteTelefone2();
            DeleteTelefoneTipado();
            Console.ReadKey();
        }

        private static void CarregarDadosDataSet()
        {
            Stopwatch sw = Stopwatch.StartNew();
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                comm.Select("select top 1000000 * from TRANSACOES_ESTOQUE");
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta DataSet - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarDadosClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<TransacoesEstoque> trans;
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                trans = comm.Select<TransacoesEstoque>("select top 1000000 * from TRANSACOES_ESTOQUE").ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarTodosDadosClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<TransacoesEstoque> trans;
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                trans = comm.GetAll<TransacoesEstoque>().ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void CarregarDadosEspecificoClasseTipada()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<TransacoesEstoque> trans;
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                trans = comm.Get<TransacoesEstoque>("ID_TRES > @ID_TRES and QTDE_TRES = @QTDE_TRES", new { ID_TRES = 15232604, QTDE_TRES = 10 }).ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }

        private static void InserirTelefone()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                Telefones telefone1 = new Telefones()
                {
                    NUMERO_FONE = "11123456789",
                    ID_PESSOA = 40,
                    OBS_FONE = "Teste de comando inserir1"
                };

                comm.Insert(telefone1);
            }
        }

        private static void InserirTelefoneBegin()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                Telefones telefone1 = new Telefones()
                {
                    NUMERO_FONE = "11123456789",
                    ID_PESSOA = 40,
                    OBS_FONE = "Teste de comando inserir1"
                };

                comm.Insert(telefone1);

                Telefones telefone2 = new Telefones()
                {
                    NUMERO_FONE = "11987654321",
                    ID_PESSOA = 40,
                    OBS_FONE = "Teste de comando inserir1"
                };

                comm.Insert(telefone2);

                comm.Commit();
            }
        }


        private static void InserirTelefoneComando()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int id = comm.Insert("insert into TELEFONES (NUMERO_FONE, OBS_FONE, ID_PESSOA) values ('11123456789', 'Teste de comando inserir', 40)");
                int id2 = comm.Insert("TELEFONES",  new { NUMERO_FONE = 11987654321, OBS_FONE = "Teste inserir", ID_PESSOA = 40 });
                comm.Commit();
            }
        }


        private static void UpdateTelefone()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                int linhasAlterada = comm.Update("update telefones set OBS_FONE = 'alteração' where ID_FONE = 1878");
            }
        }

        private static void UpdateTelefoneBegin()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comm.Update("update telefones set OBS_FONE = 'alteração2' where ID_FONE = 1878");
                comm.Commit();
            }
        }

        private static void UpdateTelefone2()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comm.Update("telefones", new { OBS_FONE = "Nova alteração" }, "ID_FONE = 1878");
                comm.Commit();
            }
        }

        private static void UpdateTelefoneTipado()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                Telefones fone = comm.Get<Telefones>("ID_FONE = @ID_FONE", new { ID_FONE = 10431 }).FirstOrDefault();

                fone.OBS_FONE = "Alteração com busca";
                int linhasAlterada = comm.Update(fone);
                comm.Commit();
            }
        }

        private static void DeleteTelefone()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                int linhasAlterada = comm.Delete("delete from telefones where ID_FONE = 10469");
            }
        }

        private static void DeleteTelefoneBegin()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comm.Delete("delete from telefones where ID_FONE = 10470");
                comm.Commit();
            }
        }

        private static void DeleteTelefone2()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                int linhasAlterada = comm.Delete("telefones", "ID_FONE = 10471");
                comm.Commit();
            }
        }

        private static void DeleteTelefoneTipado()
        {
            using (ComandoSQL comm = con.AbrirComandoSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
            {
                Telefones fone = comm.Get<Telefones>("ID_FONE = @ID_FONE", new { ID_FONE = 10474 }).FirstOrDefault();

                int linhasAlterada = comm.Delete(fone);
                comm.Commit();
            }
        }
    }
}