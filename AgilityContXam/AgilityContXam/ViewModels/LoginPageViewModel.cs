using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Net;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly IPageDialogService _dialogService;

        private LoginBindingModel _loginBindingModel;
        public LoginBindingModel LoginBindingModel
        {
            get => _loginBindingModel;
            set => SetProperty(ref _loginBindingModel, value);
        }

        public Command LoginCommand { get; }

        public LoginPageViewModel(INavigationService navigationService,
                                  IPageDialogService dialogService,
                                  IAccountService accountService)
            : base(navigationService)
        {
            _accountService = accountService;
            _dialogService = dialogService;

            LoginBindingModel = new LoginBindingModel();

            LoginCommand = new Command(ExecuteLoginCommand);
        }

        private async void ExecuteLoginCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(LoginBindingModel.Username))
            {
                await _dialogService.DisplayAlertAsync("Ops", "Informe um email", "OK");
                return;
            }
            if (string.IsNullOrEmpty(LoginBindingModel.Password))
            {
                await _dialogService.DisplayAlertAsync("Ops", "Informe uma senha", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var accesstoken = await _accountService.LoginAsync(LoginBindingModel.Username, LoginBindingModel.Password);

                if (accesstoken == null)
                {
                    IsBusy = false;
                    await _dialogService.DisplayAlertAsync("Ops", "Login e/ou senha inválidos", "OK");
                    return;
                }

                await _accountService.GetAccount(Priority.UserInitiated, true);

                await NavigationService.NavigateAsync("/Nav/MainPage");
            }
            catch (WebException)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Ocorreu um erro na conexão, verifique sua conexão com a internet", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Ocorreu um erro na operação. " + ex.Message, "OK");
            }

            IsBusy = false;
        }
    }
}
