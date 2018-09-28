using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [TabelaDB(Nome = "TELEFONES")]
    public class Telefones
    {
        [CampoDB(ChavePrimaria = true, Incremento =true)]
        public int ID_FONE { get; set; }
        public string NUMERO_FONE { get; set; }
        public string OBS_FONE { get; set; }
        public int ID_PESSOA { get; set; }
    }
}
