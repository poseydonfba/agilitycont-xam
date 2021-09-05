using AgilityContXam.Models;
using Fusillade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface ITipoDespesaService
    {
        Task<IEnumerable<TipoDespesa>> ObterTodosAsync(Priority priority, bool forceRefresh = false);
    }
}
