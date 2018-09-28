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
            GerarString();
            Console.ReadKey();
        }

        private static void GerarString()
        {
            object param = new
            {
                Valor = 0,
                Valor1 = 12,
                Valor2 = "teste"
            };

            string juncao = param.JuntarParametro(", ");
        }


        private static void CarregarDadosDataSet()
        {
            Stopwatch sw = Stopwatch.StartNew();
            using (ComandoSQL comm = con.AbrirComandoSQL())
            {
                comm.Consultar("select top 1000000 * from TRANSACOES_ESTOQUE");
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
                trans = comm.Consultar<TransacoesEstoque>("select top 1000000 * from TRANSACOES_ESTOQUE").ToList();
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
                trans = comm.GetTodos<TransacoesEstoque>().ToList();
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
                trans = comm.Get<TransacoesEstoque>("where ID_TRES > @ID_TRES and QTDE_TRES = @QTDE_TRES", new { ID_TRES = 15232604, QTDE_TRES = 10 }).ToList();
            }
            sw.Stop();
            Console.WriteLine($"Tempo de retorno da consulta Classe tipada - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}");
        }
    }
}