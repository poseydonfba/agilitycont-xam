using AgilityContXam.Enums;
using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        public Task Initialization { get; }

        private Usuario _account;
        public Usuario Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private IList<MainMenu> _menus;
        public IList<MainMenu> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        public Command<MainMenuType> NavigateToCommand { get; private set; }

        public MenuPageViewModel(INavigationService navigationService,
                                 IAccountService accountService)
            : base(navigationService)
        {
            _accountService = accountService;

            NavigateToCommand = new Command<MainMenuType>(async (type) => await NavigateToAsync(type));

            Initialization = InitializationAsync();

            Account = Settings.Usuario;

            MessagingCenter.Subscribe<MainPageViewModel, string>(this, "AlterarFoto", (obj, base64) =>
            {
                Account.Foto = base64;
            });
        }

        private async Task InitializationAsync()
        {
            await IsAuthenticated();
        }

        private async Task IsAuthenticated()
        {
            if (CheckIfJwtIsEmpty() || CheckIfJwtIsExpired())
            {
                //await NavigationService.NavigateAsync("/LoginPage");
                return;
            }

            await LoadAccountAsync();
            await LoadMenusAsync();
        }

        private async Task LoadAccountAsync()
        {
            Account = await _accountService.GetAccount(Priority.Background, true);
            Account = Settings.Usuario;
        }

        private async Task LoadMenusAsync()
        {
            var menus = await _accountService.GetMenus();
            Menus = new List<MainMenu>(menus);
        }

        private async Task NavigateToAsync(MainMenuType type)
        {
            System.Diagnostics.Debug.WriteLine(type);

            var masterDetail = Application.Current.MainPage as MasterDetailPage;
            masterDetail.IsPresented = false;

            switch (type)
            {
                case MainMenuType.Transacao:
                    await NavigationService.NavigateAsync("TransacaoPage");
                    break;
                case MainMenuType.Logout:
                    _accountService.LogoutAsync();
                    await NavigationService.NavigateAsync("/LoginPage");
                    break;
            }
        }
    }
}
