using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [TabelaDB(Nome = "CLIENTES")]
    public class Cliente
    {
        [CampoDB(ChavePrimaria = true, Incremento =true)]
        public int ID_CLI { get; set; }
        public string NOME_CLI { get; set; }
        public string CPF_CLI { get; set; }
        public DateTime DATANASC_CLI { get; set; }
        public string TELEFONE_CLI { get; set; }
    }
}
