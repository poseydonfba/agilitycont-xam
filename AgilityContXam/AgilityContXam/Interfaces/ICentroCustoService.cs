using AgilityContXam.Models;
using Fusillade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface ICentroCustoService
    {
        Task<IEnumerable<CentroCusto>> ObterTodosAsync(Priority priority, bool forceRefresh = false);
    }
}
