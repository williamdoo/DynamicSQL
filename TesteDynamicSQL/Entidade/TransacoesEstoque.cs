using DynamicSQL.Extencoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDynamicSQL.Entidade
{
    [TabelaDB(Nome = "TRANSACOES_ESTOQUE")]
    public class TransacoesEstoque
    {
        [CampoDB(Primarykey = true, Incremento = true)]
        public int ID_TRES { get; set; }
        public int ID_OPER { get; set; }
        public int? COD_FORN { get; set; }
        public int? COD_CLI { get; set; }
        public int COD_LCES { get; set; }
        public int COD_TPMV { get; set; }
        public int ID_PRVL { get; set; }
        public DateTime DATA_TRES { get; set; }
        public decimal QTDE_TRES { get; set; }
        public decimal ESTANT_TRES { get; set; }
        public decimal ESTATU_TRES { get; set; }
        public decimal VALOR_TRES { get; set; }
        public string DOC_TRES { get; set; }
        public string OPERACAO_TRES { get; set; }
        public int? ID_ITEN { get; set; }
        public int? ID_ITVD { get; set; }
        public DateTime? DATASIS { get; set; }
        public int COD_LOJA { get; set; }
        public int? ID_ITVI { get; set; }
        public int? ID_TEST { get; set; }
        public DateTime? DATACOM_TRES { get; set; }
        public string FLAG_TRES { get; set; }
    }
}
