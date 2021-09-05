using Prism;
using Prism.Ioc;
using AgilityContXam.ViewModels;
using AgilityContXam.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AgilityContXam.Helpers;
using System;
using AgilityContXam.Interfaces;
using AgilityContXam.Services;
using System.Threading.Tasks;
using System.IO;
using AgilityContXam.DataStore;
using Plugin.Connectivity;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AgilityContXam
{
    public partial class App
    {
        public static double Height { get; set; }
        public static double Width { get; set; }

        static SQLiteHelper db;
        public static SQLiteHelper SQLiteDb
        {
            get
            {
                if (db == null)
                {
                    //db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "agilitycontdb.db3"));
                    db = new SQLiteHelper(Path.Combine(FileSystem.AppDataDirectory, "agilitycontdb.db3"));
                }
                return db;
            }
        }

        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            //await NavigationService.NavigateAsync("NavigationPage/MainPage");
            //await NavigationService.NavigateAsync("Master/Nav/MainPage");
            //await NavigationService.NavigateAsync("Nav/MainPage");
            await NavigationService.NavigateAsync("Nav/MainPage");

#if DEBUG

            HotReloader.Current.Run(this);
            //HotReloader.Current.Run(this, new HotReloader.Configuration
            //{
            //    DeviceUrlPort = 8000,
            //    ExtensionIpAddress = 15000
            //});

#endif

            //if (!string.IsNullOrEmpty(Settings.AccessToken))
            //{
            //    if (Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1))
            //        await NavigationService.NavigateAsync("/LoginPage");
            //    else
            //        await NavigationService.NavigateAsync("/Master/Nav/MainPage");
            //}
            //else
            //    await NavigationService.NavigateAsync("LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<PrismMasterDetailPage, PrismMasterDetailPageViewModel>("Master");
            containerRegistry.RegisterForNavigation<PrismNavigationPage, PrismNavigationPageViewModel>("Nav");
            containerRegistry.RegisterForNavigation<MenuPage, MenuPageViewModel>();
            containerRegistry.RegisterForNavigation<TransacaoPage, TransacaoPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<NovaTransacaoPage, NovaTransacaoPageViewModel>();

            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<IAccountService, AccountService>();
            containerRegistry.Register<ICentroCustoService, CentroCustoService>();
            containerRegistry.Register<IFormaPagamentoService, FormaPagamentoService>();
            containerRegistry.Register<ITipoDespesaService, TipoDespesaService>();
            containerRegistry.Register<ITipoReceitaService, TipoReceitaService>();
            containerRegistry.Register<ITransacaoService, TransacaoService>();
            containerRegistry.RegisterForNavigation<RelSaldoPage, RelSaldoPageViewModel>();
            containerRegistry.RegisterForNavigation<UIPage, UIPageViewModel>();
        }
    }
}
