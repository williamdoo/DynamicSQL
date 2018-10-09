using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [Table(Name = "CLIENTES")]
    public class Cliente
    {
        [Column(Primarykey =  true, Increment  =true)]
        public int ID_CLI { get; set; }
        public string NOME_CLI { get; set; }
        public string CPF_CLI { get; set; }
        public DateTime DATANASC_CLI { get; set; }
        public string TELEFONE_CLI { get; set; }
    }
}
