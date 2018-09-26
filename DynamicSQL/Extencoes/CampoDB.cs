using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSQL.Extencoes
{
    public class CampoDB : Attribute
    {
        public bool Primarykey { get; set; } = true;
        public bool Incremento { get; set; } = true;

        public CampoDB()
        {

        }
    }
}
