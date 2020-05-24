using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [Table("Pessoa")]
    public class Pessoa:MapEntity
    {
        [PrimaryKey(Increment =true)]
        public int Id_Pess { get; set; }
        public string Nome_Pess { get; set; }
        public IList<Loja> Lojas { get; set; }
        public IList<Email> Emails { get; set; }
    }
}
