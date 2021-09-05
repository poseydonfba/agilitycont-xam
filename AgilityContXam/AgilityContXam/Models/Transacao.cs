using SQLite;
using System;

namespace AgilityContXam.Models
{
    public class Transacao
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int IdTipoLancamento { get; set; }
        public Guid IdTipoTransacao { get; set; }
        public string Descricao { get; set; }
        public DateTime DataTransacao { get; set; }
        public double Valor { get; set; }
        public double Desconto { get; set; }
        public Guid IdFormaPagamento { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid? Uer { get; set; }
        public DateTime? Der { get; set; }

        public string DescTipoLancamento { get; set; }
        public string DescTipoTransacao { get; set; }
        public string DescFormaPagamento { get; set; }
    }
}
