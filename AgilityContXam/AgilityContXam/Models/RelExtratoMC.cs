using System.Collections.ObjectModel;

namespace AgilityContXam.Models
{
    public class RelExtratoMC
    {
        public ObservableCollection<Transacao> Transacoes { get; set; }
        public double Saldo { get; set; }
    }
}
