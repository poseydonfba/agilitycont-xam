using System;
using System.Net.Http;
using Fusillade;
using AgilityContXam.Interfaces;
using ModernHttpClient;
using Refit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AgilityContXam.Services.ApiService))]
namespace AgilityContXam.Services
{
    public class ApiService : IApiService
    {
        public ApiService()
        {
            Func<HttpMessageHandler, IApi> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(Constants.BaseApiAddressVersion)
                };

                return RestService.For<IApi>(client);
            };

            _background = new Lazy<IApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Background)));

            _userInitiated = new Lazy<IApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<IApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Speculative)));
        }

        private readonly Lazy<IApi> _background;
        private readonly Lazy<IApi> _userInitiated;
        private readonly Lazy<IApi> _speculative;

        public IApi Background
        {
            get { return _background.Value; }
        }

        public IApi UserInitiated
        {
            get { return _userInitiated.Value; }
        }

        public IApi Speculative
        {
            get { return _speculative.Value; }
        }
    }
}
