using SQLite;
using System;

namespace AgilityContXam.Models
{
    public class TipoReceita
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public DateTime? Der { get; set; }
    }
}
