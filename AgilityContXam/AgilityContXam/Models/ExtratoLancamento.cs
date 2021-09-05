using SQLite;
using System;

namespace AgilityContXam.Models
{
    public class ExtratoLancamento
    {
        [PrimaryKey]
        public string Id { get; set; }
        public Guid IdUsuario { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public string DescMes { get; set; }
        public double TotalReceita { get; set; }
        public double TotalDespesa { get; set; }
    }
}
