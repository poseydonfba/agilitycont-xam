using AgilityContXam.Enums;
using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AgilityContXam.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApiService _apiService;

        public AccountService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            var isConnected = CrossConnectivity.Current.IsConnected;
            if (!isConnected)
                throw new Exception("Sem acesso a internet");

            var isRemoteReachable = await CrossConnectivity.Current.IsRemoteReachable(new Uri(Constants.BaseApiAddress), TimeSpan.FromSeconds(5));
            if (!isRemoteReachable)
                throw new Exception("Falha na comunicação com o servidor");

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post, Constants.BaseApiAddress + "Token");

            request.Content = new FormUrlEncodedContent(keyValues);

            using (var handler = new ModernHttpClient.NativeMessageHandler())
            using (var client = new HttpClient(handler))
            {
                var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    JObject jwtDynamic = JsonConvert.DeserializeObject<JObject>(content);

                    var expiresIn = Convert.ToDouble(jwtDynamic.Value<double>("expires_in"));
                    var accessTokenExpiration = DateTime.Now + TimeSpan.FromSeconds(expiresIn);
                    var accessToken = jwtDynamic.Value<string>("access_token");

                    Settings.AccessTokenExpirationDate = accessTokenExpiration;
                    Settings.AccessToken = accessToken;

                    return accessToken;
                }
                //else
                //{
                //    throw new Exception(response.ReasonPhrase);
                //}
            }

            return null;
        }

        public async Task<Usuario> GetAccount(Priority priority, bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                var usuario = await GetRemoteAccountAsync(priority);
                if (usuario != null)
                {
                    Settings.Id = usuario.Id;
                    Settings.Nome = usuario.Nome;
                    Settings.Email = usuario.Email;
                    Settings.Chave = usuario.Chave;
                    Settings.Ativo = usuario.Ativo;
                    Settings.Tipo = usuario.Tipo;
                    Settings.Foto = usuario.Foto;
                }
            }

            return Settings.Usuario;
        }
        async Task<Usuario> GetRemoteAccountAsync(Priority priority)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Usuario result = null;
            Task<Usuario> getTask;
            switch (priority)
            {
                case Priority.Background:
                    getTask = _apiService.Background.GetAccount(apiToken);
                    break;
                case Priority.UserInitiated:
                    getTask = _apiService.UserInitiated.GetAccount(apiToken);
                    break;
                case Priority.Speculative:
                    getTask = _apiService.Speculative.GetAccount(apiToken);
                    break;
                default:
                    getTask = _apiService.UserInitiated.GetAccount(apiToken);
                    break;
            }

            var isConnected = CrossConnectivity.Current.IsConnected;
            if (!isConnected)
                throw new Exception("Sem acesso a internet");

            var isRemoteReachable = await CrossConnectivity.Current.IsRemoteReachable(new Uri(Constants.BaseApiAddress), TimeSpan.FromSeconds(5));
            if (!isRemoteReachable)
                throw new Exception("Falha na comunicação com o servidor");

            result = await Policy
                  .Handle<WebException>()
                  .WaitAndRetryAsync
                  (
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                  )
                  .ExecuteAsync(async () => await getTask);

            return result;
        }

        public void LogoutAsync()
        {
            Settings.Id = Guid.Empty;
            Settings.Nome = string.Empty;
            Settings.Email = string.Empty;
            Settings.Chave = string.Empty;
            Settings.Ativo = 0;
            Settings.Tipo = 0;
            Settings.Foto = string.Empty;
            Settings.Username = string.Empty;
            Settings.AccessTokenExpirationDate = DateTime.UtcNow;
            Settings.AccessToken = string.Empty;
        }

        public Task<List<MainMenu>> GetMenus()
        {
            var menus = new List<MainMenu> {
                    new MainMenu{
                        Name="Transações",
                        Order = 1,
                        Type = MainMenuType.Transacao,
                        Icon = "menu_wallet.png",
                        //Color = "#00c6ae"
                        // Color = "#e8eaf6"
                        Color = "#FFFFFF"
                    },
                    new MainMenu{
                        Name="Termos",
                        Order = 2,
                        Type = MainMenuType.TermsConditions,
                        Icon = "menu_wallet.png",
                        //Color = "#f3f5f9"
                        //Color = "#c5cae9"
                        Color = "#FFFFFF"
                    },
                    new MainMenu{
                        Name="Ajuda",
                        Order = 3,
                        Type = MainMenuType.HelpCenter,
                        Icon = "menu_wallet.png",
                        //Color = "#f3f5f9"
                        //Color = "#e3f2fd"
                        Color = "#FFFFFF"
                    },
                    new MainMenu{
                        Name="Sair",
                        Order = 4,
                        Type = MainMenuType.Logout,
                        Icon = "menu_logout.png",
                        //Color = "#FFFFFF"
                        //Color = "#bbdefb"
                        Color = "#FFFFFF"
                    }
                };

            return Task.FromResult(menus);
        }

        public async Task AlterarFoto(AlterarFotoBindingModel foto)
        {
            string apiToken = $"Bearer {Settings.AccessToken}";
            Task postTask = _apiService.UserInitiated.AlterarFoto(foto, apiToken);

            var isConnected = CrossConnectivity.Current.IsConnected;
            if (!isConnected)
                throw new Exception("Sem acesso a internet");

            var isRemoteReachable = await CrossConnectivity.Current.IsRemoteReachable(new Uri(Constants.BaseApiAddress), TimeSpan.FromSeconds(5));
            if (!isRemoteReachable)
                throw new Exception("Falha na comunicação com o servidor");

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
}
