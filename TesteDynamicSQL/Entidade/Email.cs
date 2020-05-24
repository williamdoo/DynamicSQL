using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [Table("Email")]
    public class Email: MapEntity
    {
        [PrimaryKey(Increment = true)]
        public int Id_Emai { get; set; }
        public int Id_Pess { get; set; }
        public string Email_Emai { get; set; }
    }
}
