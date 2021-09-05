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
    public class TipoReceitaService : ITipoReceitaService
    {
        private readonly IApiService _apiService;

        public TipoReceitaService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<TipoReceita>> ObterTodosAsync(Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteTipoReceitaAsync(priority);
        }
        async Task<List<TipoReceita>> GetRemoteTipoReceitaAsync(Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<TipoReceita> result = null;
            Task<List<TipoReceita>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllTipoReceita(apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllTipoReceita(apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllTipoReceita(apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllTipoReceita(apiToken);
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
