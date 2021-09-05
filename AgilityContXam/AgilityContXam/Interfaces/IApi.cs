using AgilityContXam.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    [Headers("Accept: application/json")]
    public interface IApi
    {
        [Get("/usuario/userinfo")]
        Task<Usuario> GetAccount([Header("Authorization")] string authorization);

        [Post("/usuario/AlterarFoto")]
        Task AlterarFoto([Body] AlterarFotoBindingModel foto, [Header("Authorization")] string authorization);


        [Get("/centrocusto")]
        Task<List<CentroCusto>> GetAllCentroCusto([Header("Authorization")] string authorization);

        [Get("/formapagamento")]
        Task<List<FormaPagamento>> GetAllFormaPagamento([Header("Authorization")] string authorization);

        [Get("/tipodespesa")]
        Task<List<TipoDespesa>> GetAllTipoDespesa([Header("Authorization")] string authorization);

        [Get("/tiporeceita")]
        Task<List<TipoReceita>> GetAllTipoReceita([Header("Authorization")] string authorization);




        [Get("/transacao/{id}")]
        Task<Transacao> GetTransacaoPorId(Guid id, [Header("Authorization")] string authorization);

        [Get("/transacao/todos/{pageIndex}/{pageSize}")]
        Task<List<Transacao>> GetAllTransacaoPorUsuarioId(int pageIndex, int pageSize, [Header("Authorization")] string authorization);

        [Get("/transacao/todos/{dataInicio}/{dataFim}/{pageIndex}/{pageSize}")]
        Task<List<Transacao>> GetAllTransacaoPorUsuarioId(string dataInicio, string dataFim, int pageIndex, int pageSize, [Header("Authorization")] string authorization);

        [Post("/transacao")]
        Task IncluirTransacao([Body] IncluirTransacaoBindingModel model, [Header("Authorization")] string authorization);

        [Post("/transacao/alterar")]
        Task AlterarTransacao([Body] AlterarTransacaoBindingModel model, [Header("Authorization")] string authorization);

        [Post("/transacao/excluir")]
        Task ExcluirTransacao([Body] ExcluirTransacaoBindingModel model, [Header("Authorization")] string authorization);



        [Get("/transacao/extratoconsolidado/{ano}")]
        Task<List<ExtratoLancamento>> GetAllExtratoConsolidado(int ano, [Header("Authorization")] string authorization);

    }
}
