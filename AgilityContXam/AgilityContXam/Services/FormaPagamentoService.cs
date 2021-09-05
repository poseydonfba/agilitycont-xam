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
    public class FormaPagamentoService : IFormaPagamentoService
    {
        private readonly IApiService _apiService;

        public FormaPagamentoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<FormaPagamento>> ObterTodosAsync(Priority priority, bool forceRefresh = false)
        {
            return await GetRemoteFormaPagamentoAsync(priority);
        }
        async Task<List<FormaPagamento>> GetRemoteFormaPagamentoAsync(Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            List<FormaPagamento> result = null;
            Task<List<FormaPagamento>> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAllFormaPagamento(apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAllFormaPagamento(apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAllFormaPagamento(apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAllFormaPagamento(apiToken);
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
