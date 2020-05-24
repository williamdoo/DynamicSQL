using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [Table("Loja")]
    public class Loja:MapEntity
    {
        [PrimaryKey(Increment = true)]
        public int Cod_Loja { get; set; }
        public int Id_Pess { get; set; }
        public string Fantasia_Loja { get; set; }
        public DateTime DataCadastro_Loja { get; set; }
        public string Ativo_Loja { get; set; }
    }
}
