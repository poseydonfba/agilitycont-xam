using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AgilityContXam.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly IApiService _apiService;

        public TransacaoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task InserirAsync(IncluirTransacaoBindingModel model)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Task postTask = _apiService.UserInitiated.IncluirTransacao(model, apiToken);

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                await Policy
                      .Handle<Exception>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 3,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await postTask);
            }
        }

        public async Task AlterarAsync(AlterarTransacaoBindingModel model)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Task postTask = _apiService.UserInitiated.AlterarTransacao(model, apiToken);

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                await Policy
                      .Handle<Exception>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 3,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await postTask);
            }
        }

        public async Task ExcluirAsync(ExcluirTransacaoBindingModel model)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Task postTask = _apiService.UserInitiated.ExcluirTransacao(model, apiToken);

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                await Policy
                      .Handle<Exception>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 3,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await postTask);
            }
        }

        public async Task<Transacao> ObterByIdAsync(Guid id, Priority priority)
        {
            return await GetRemoteTransacaoAsync(id, priority);
        }
        async Task<Transacao> GetRemoteTransacaoAsync(Guid id, Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Transacao result = null;
            Task<Transacao> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetTransacaoPorId(id, apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetTransacaoPorId(id, apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetTransacaoPorId(id, apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetTransacaoPorId(id, apiToken);
                    break;
            }

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                result = await Policy
                      .Handle<WebException>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await getTask);
            }
            return result;
        }

        public async Task<IEnumerable<Transacao>> ObterTodosPorUsuarioIdAsync(int pageIndex, int pageSize, Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteTransacoesAsync(pageIndex, pageSize, priority);
        }
        async Task<List<Transacao>> GetRemoteTransacoesAsync(int pageIndex, int pageSize, Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<Transacao> result = null;
            Task<List<Transacao>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllTransacaoPorUsuarioId(pageIndex, pageSize, apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllTransacaoPorUsuarioId(pageIndex, pageSize, apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllTransacaoPorUsuarioId(pageIndex, pageSize, apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllTransacaoPorUsuarioId(pageIndex, pageSize, apiToken);
                    break;
            }

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                result = await Policy
                      .Handle<WebException>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await getTask);
            }
            return result;
        }

        public async Task<IEnumerable<Transacao>> ObterTodosPorUsuarioIdAsync(DateTime dataInicio, DateTime dataFim, int pageIndex, int pageSize, Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteTransacoesAsync(dataInicio, dataFim, pageIndex, pageSize, priority);
        }
        async Task<List<Transacao>> GetRemoteTransacoesAsync(DateTime dataInicio, DateTime dataFim, int pageIndex, int pageSize, Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<Transacao> result = null;
            Task<List<Transacao>> getTask;
            var inicio = dataInicio.ToString("yyyyMMddHHmmss");
            var fim = dataFim.ToString("yyyyMMddHHmmss");
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllTransacaoPorUsuarioId(inicio, fim, pageIndex, pageSize, apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllTransacaoPorUsuarioId(inicio, fim, pageIndex, pageSize, apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllTransacaoPorUsuarioId(inicio, fim, pageIndex, pageSize, apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllTransacaoPorUsuarioId(inicio, fim, pageIndex, pageSize, apiToken);
                    break;
            }

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                result = await Policy
                      .Handle<WebException>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await getTask);
            }
            return result;
        }

        public async Task<IEnumerable<ExtratoLancamento>> ObterExtratoConsolidadoAsync(int ano, Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteExtratoConsolidadoAsync(ano, priority);
        }
        async Task<List<ExtratoLancamento>> GetRemoteExtratoConsolidadoAsync(int ano, Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<ExtratoLancamento> result = null;
            Task<List<ExtratoLancamento>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllExtratoConsolidado(ano, apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllExtratoConsolidado(ano, apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllExtratoConsolidado(ano, apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllExtratoConsolidado(ano, apiToken);
                    break;
            }

            var isConnected = await InternetConnection.IsConnected();

            if (isConnected)
            {
                result = await Policy
                      .Handle<WebException>()
                      .WaitAndRetryAsync
                      (
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      )
                      .ExecuteAsync(async () => await getTask);
            }
            return result;
        }
    }
}
