using AgilityContXam.Models;
using Fusillade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface IFormaPagamentoService
    {
        Task<IEnumerable<FormaPagamento>> ObterTodosAsync(Priority priority, bool forceRefresh = false);
    }
}
