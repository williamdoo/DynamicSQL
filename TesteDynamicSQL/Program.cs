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
        private static Connection conexao;
        static void Main(string[] args)
        {
            conexao = new Connection("Data Source=WILLIAM-NB;Database=VegasDesenv;Integrated Security=true;");
            TestarEntidadeLojaERelacionamento();
            Console.ReadKey();
        }

        private static void TestarEntidadeLojaERelacionamento()
        {
            using (CommandSQL comm = conexao.OpenCommandSQL())
            {
                Pessoa pessoa = comm.Get<Pessoa>(1);
            }
        }
    }
}