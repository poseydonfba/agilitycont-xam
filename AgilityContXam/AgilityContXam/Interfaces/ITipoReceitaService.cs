using AgilityContXam.Models;
using Fusillade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface ITipoReceitaService
    {
        Task<IEnumerable<TipoReceita>> ObterTodosAsync(Priority priority, bool forceRefresh = false);
    }
}
