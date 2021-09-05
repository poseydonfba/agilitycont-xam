using AgilityContXam.Models;
using Fusillade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface ITransacaoService
    {
        Task InserirAsync(IncluirTransacaoBindingModel model);

        Task AlterarAsync(AlterarTransacaoBindingModel model);

        Task ExcluirAsync(ExcluirTransacaoBindingModel model);

        Task<Transacao> ObterByIdAsync(Guid id, Priority priority);

        Task<IEnumerable<Transacao>> ObterTodosPorUsuarioIdAsync(int pageIndex, int pageSize, Priority priority, bool forceRefresh = false);

        Task<IEnumerable<Transacao>> ObterTodosPorUsuarioIdAsync(DateTime dataInicio, DateTime dataFim, int pageIndex, int pageSize, Priority priority, bool forceRefresh = false);
        
        Task<IEnumerable<ExtratoLancamento>> ObterExtratoConsolidadoAsync(int ano, Priority priority, bool forceRefresh = false);
    }
}
