using SQLite;
using System;

namespace AgilityContXam.Models
{
    public class FormaPagamento
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public Guid? Uer { get; set; }
        public DateTime? Der { get; set; }
    }
}
