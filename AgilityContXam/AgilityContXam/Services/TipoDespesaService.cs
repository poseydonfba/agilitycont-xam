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
    public class TipoDespesaService : ITipoDespesaService
    {
        private readonly IApiService _apiService;

        public TipoDespesaService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<TipoDespesa>> ObterTodosAsync(Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteTipoDespesaAsync(priority);
        }
        async Task<List<TipoDespesa>> GetRemoteTipoDespesaAsync(Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<TipoDespesa> result = null;
            Task<List<TipoDespesa>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllTipoDespesa(apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllTipoDespesa(apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllTipoDespesa(apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllTipoDespesa(apiToken);
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
