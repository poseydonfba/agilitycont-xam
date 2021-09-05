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
    public class CentroCustoService : ICentroCustoService
    {
        private readonly IApiService _apiService;

        public CentroCustoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<CentroCusto>> ObterTodosAsync(Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteCentroCustoAsync(priority);
        }
        async Task<List<CentroCusto>> GetRemoteCentroCustoAsync(Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<CentroCusto> result = null;
            Task<List<CentroCusto>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllCentroCusto(apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllCentroCusto(apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllCentroCusto(apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllCentroCusto(apiToken);
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
