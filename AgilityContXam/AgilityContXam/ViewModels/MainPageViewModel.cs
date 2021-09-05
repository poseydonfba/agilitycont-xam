using AgilityContXam.Enums;
using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly IAccountService _accountService;
        private readonly ICentroCustoService _centroCustoService;
        private readonly IFormaPagamentoService _formaPagamentoService;
        private readonly ITipoDespesaService _tipoDespesaService;
        private readonly ITipoReceitaService _tipoReceitaService;
        private readonly ITransacaoService _transacaoService;

        public ObservableCollection<ExtratoLancamento> ExtratoLancamentos { get; set; }

        private IList<MainMenu> _menus;
        public IList<MainMenu> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        private Usuario _account;
        public Usuario Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        object _selectedMenu;
        public object SelectedMenu
        {
            get => _selectedMenu;
            set => SetProperty(ref _selectedMenu, value);
        }

        private int _carouselPosition;

        public int CarouselPosition
        {
            get { return _carouselPosition; }
            set { _carouselPosition = value; RaisePropertyChanged(); }
        }


        public Command OpenCameraCommand { get; }
        public Command OpenLibraryCommand { get; }
        public Command MenuSelectionChangedCommand { get; }

        public Command OpenTransactionCommand { get; }
        public Command OpenExtratoCommand { get; }
        public Command LogoutCommand { get; }

        public Command SlideOpenCommand { get; }
        public Command SlideCloseCommand { get; }

        public MainPageViewModel(INavigationService navigationService,
                                 IPageDialogService dialogService,
                                 IAccountService accountService,
                                 ICentroCustoService centroCustoService,
                                 IFormaPagamentoService formaPagamentoService,
                                 ITipoDespesaService tipoDespesaService,
                                 ITipoReceitaService tipoReceitaService,
                                 ITransacaoService transacaoService)
            : base(navigationService)
        {
            _dialogService = dialogService;
            _accountService = accountService;
            _centroCustoService = centroCustoService;
            _formaPagamentoService = formaPagamentoService;
            _tipoDespesaService = tipoDespesaService;
            _tipoReceitaService = tipoReceitaService;
            _transacaoService = transacaoService;

            Title = Constants.AppName;

            Account = Settings.Usuario;

            IsSlide = false;

            OpenCameraCommand = new Command(ExecuteOpenCameraCommand);
            OpenLibraryCommand = new Command(ExecuteOpenLibraryCommand);
            MenuSelectionChangedCommand = new Command(ExecuteMenuSelectionChangedCommand);
            OpenTransactionCommand = new Command(ExecuteOpenTransactionCommand);
            OpenExtratoCommand = new Command(ExecuteOpenExtratoCommand);
            LogoutCommand = new Command(ExecuteLogoutCommand);
            SlideOpenCommand = new Command(ExecuteSlideOpenCommand);
            SlideCloseCommand = new Command(ExecuteSlideCloseCommand);

            ExtratoLancamentos = new ObservableCollection<ExtratoLancamento>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            await IsAuthenticated();
        }

        private async Task IsAuthenticated()
        {
            if (CheckIfJwtIsEmpty() || CheckIfJwtIsExpired())
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await NavigationService.NavigateAsync("/LoginPage");
                });
                return;
            }

            await CreateFolders();

            await LoadMenusAsync();
            await LoadAccountAsync();
            await LoadTransacoesExtrato();
            await GetRemoteBaseData();
        }

        private async Task LoadAccountAsync()
        {
            try
            {
                Account = await _accountService.GetAccount(Priority.Background, true);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Falha ao carregar perfil. " + ex.Message, "Ok");
            }
        }

        private async Task LoadMenusAsync()
        {
            try
            {
                Menus = await _accountService.GetMenus();
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Falha ao carregar menu. " + ex.Message, "Ok");
            }
        }

        private async Task LoadTransacoesExtrato()
        {
            await GetLocalExtratoLancamentos();
            await GetRemoteExtratoLancamentos();
            await GetLocalExtratoLancamentos();
        }
        private async Task GetRemoteExtratoLancamentos()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await _transacaoService
                    .ObterExtratoConsolidadoAsync(DateTime.Today.Year, Priority.UserInitiated, true);

                if (items != null)
                    await App.SQLiteDb.ExtratoLancamento.SaveAllAsync(items.ToList());

                IsBusy = false;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task GetLocalExtratoLancamentos()
        {
            try
            {
                ExtratoLancamentos.Clear();

                var items = await App.SQLiteDb.ExtratoLancamento.GetAsync(DateTime.Today.Year);

                if (!items.Any())
                {
                    items.Add(new ExtratoLancamento
                    {
                        Ano = DateTime.Today.Year,
                        Mes = DateTime.Today.Month,
                        TotalReceita = 0,
                        TotalDespesa = 0
                    });
                }

                ExtratoLancamentos.AddRange(items);

                CarouselPosition = DateTime.Today.Month - 1;
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async Task GetRemoteBaseData()
        {
            try
            {
                var result1 = await _centroCustoService.ObterTodosAsync(Priority.Background, true);
                if (result1 != null)
                    await App.SQLiteDb.SaveAllCentroCustoAsync(result1.ToList());

                var result2 = await _formaPagamentoService.ObterTodosAsync(Priority.Background, true);
                if (result2 != null)
                    await App.SQLiteDb.SaveAllFormaPagamentoAsync(result2.ToList());

                var result3 = await _tipoDespesaService.ObterTodosAsync(Priority.Background, true);
                if (result3 != null)
                    await App.SQLiteDb.SaveAllTipoDespesaAsync(result3.ToList());

                var result4 = await _tipoReceitaService.ObterTodosAsync(Priority.Background, true);
                if (result4 != null)
                    await App.SQLiteDb.SaveAllTipoReceitaAsync(result4.ToList());
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Falha ao carregar base. " + ex.Message, "Ok");
            }
        }

        //private async Task UnableToLoad(Exception ex)
        //{
        //    await _dialogService.DisplayAlertAsync("Ops", ex.Message, "Ok");
        //    await NavigationService.NavigateAsync("/LoginPage");
        //}

        private async void ExecuteOpenLibraryCommand()
        {
            await CrossMedia.Current.Initialize();

            IsSlide = false;

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Galeria de fotos não suportada.", "Ok");

                return;
            }

            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null)
                    return;

                using (Stream stream = file.GetStream())
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    string base64 = Convert.ToBase64String(bytes);

                    Account.Foto = base64;

                    await AlterarFoto(base64);

                    Settings.Foto = base64;

                    MessagingCenter.Send(this, "AlterarFoto", base64);
                }
            }
            catch (Exception)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Galeria de fotos não suportada.", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteOpenCameraCommand()
        {
            await CrossMedia.Current.Initialize();

            IsSlide = false;

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", "Nenhuma câmera detectada.", "Ok");

                return;
            }

            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(
                  new StoreCameraMediaOptions
                  {
                      SaveToAlbum = true,
                      Directory = Constants.AppName,
                      Name = "perfil.jpg",
                      CompressionQuality = 30
                  });

                if (file == null)
                    return;

                //await _dialogService.DisplayAlertAsync("File Location", file.Path, "OK");

                using (Stream stream = file.GetStream())
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    string base64 = Convert.ToBase64String(bytes);

                    Account.Foto = base64;

                    await AlterarFoto(base64);

                    Settings.Foto = base64;

                    MessagingCenter.Send(this, "AlterarFoto", base64);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AlterarFoto(string foto)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                await _accountService.AlterarFoto(new AlterarFotoBindingModel { Foto = foto });
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Falha ao salvar foto. " + ex.Message, "Ok");
            }

            IsBusy = false;
        }

        private async void ExecuteMenuSelectionChangedCommand()
        {
            if (SelectedMenu == null)
                return;

            MainMenu type = SelectedMenu as MainMenu;

            switch (type.Type)
            {
                case MainMenuType.Transacao:
                    await NavigationService.NavigateAsync("TransacaoPage");
                    break;
                case MainMenuType.Logout:
                    _accountService.LogoutAsync();
                    await NavigationService.NavigateAsync("/LoginPage");
                    break;
            }

            SelectedMenu = null;
        }

        private async void ExecuteOpenTransactionCommand()
        {
            await NavigationService.NavigateAsync("TransacaoPage");
        }

        private async void ExecuteOpenExtratoCommand()
        {
            await NavigationService.NavigateAsync("RelSaldoPage");
        }

        private async void ExecuteLogoutCommand()
        {
            _accountService.LogoutAsync();
            await NavigationService.NavigateAsync("/LoginPage");
        }

        private void ExecuteSlideOpenCommand()
        {
            if (IsSlide)
            {
                IsSlide = false;
            }
            else
            {
                IsSlide = true;
            }
        }

        private void ExecuteSlideCloseCommand()
        {
            IsSlide = false;
        }

        public override bool OnBackButtonPressed()
        {
            if (IsSlide)
            {
                IsSlide = false;
                return true;
            }

            return false;
        }

        public async Task CreateFolders()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    // Verifica se o usuario ja negou a permissão uma vez
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await _dialogService.DisplayAlertAsync("Ops", "Precisamos desta permissão para salvar o PDF no dispositivo.", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    Xamarin.Forms.DependencyService.Get<IFileService>().CreateFolder("Pdf");
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await _dialogService.DisplayAlertAsync("Ops", "Não pode continuar, tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
    }
}
