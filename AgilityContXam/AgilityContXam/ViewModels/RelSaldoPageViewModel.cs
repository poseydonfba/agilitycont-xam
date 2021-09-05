using AgilityContXam.Enums;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class RelSaldoPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly ITransacaoService _transacaoService;

        public ObservableCollection<Transacao> Transacoes { get; }

        public Command OpenMenuFiltroCommand { get; }
        public Command PesquisarCommand { get; }

        DateTime _dataInicio = DateTime.Today;
        public DateTime DataInicio
        {
            get => _dataInicio;
            set
            {
                _dataInicio = value;
                RaisePropertyChanged();
            }
        }

        DateTime _dataFim = DateTime.Today;
        public DateTime DataFim
        {
            get => _dataFim;
            set
            {
                _dataFim = value;
                RaisePropertyChanged();
            }
        }

        private double _saldo = 0;

        public double Saldo
        {
            get => _saldo;
            set { _saldo = value; RaisePropertyChanged(); }
        }


        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; RaisePropertyChanged(); }
        }

        public RelSaldoPageViewModel(INavigationService navigationService,
                                      IPageDialogService dialogService,
                                      ITransacaoService transacaoService)
            : base(navigationService)
        {
            _dialogService = dialogService;
            _transacaoService = transacaoService;

            Title = "Extrato";

            IsSlide = false;
            IsVisible = false;

            Device.BeginInvokeOnMainThread(() =>
            {
                IsSlide = false;
            });

            OpenMenuFiltroCommand = new Command(ExecuteOpenMenuFiltroCommand);
            PesquisarCommand = new Command(ExecutePesquisarCommand);

            Transacoes = new ObservableCollection<Transacao>();
        }

        public override void Initialize(INavigationParameters parameters)
        {
            IsVisible = !Transacoes.Any();
        }

        private async Task GetLocalTransacoes(bool clear = true)
        {
            try
            {
                if (clear)
                    Transacoes.Clear();

                var items = await App.SQLiteDb.Transacao.GetAsync(DataInicio, DataFim, 1, 50);
                items.ForEach(x => {
                    x.Valor = x.IdTipoLancamento == 3 ? -1 * x.Valor : x.Valor;
                });

                Saldo = items.Sum(x => x.Valor);

                Transacoes.AddRange(items);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async Task GetRemoteTransacoes()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await _transacaoService
                    .ObterTodosPorUsuarioIdAsync(DataInicio, DataFim, 1, 50, Priority.UserInitiated, true);

                if (items != null)
                    await App.SQLiteDb.Transacao.SaveAllAsync(items.ToList());

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

        private async void ExecutePesquisarCommand()
        {
            IsSlide = false;

            //await GetLocalTransacoes();
            await GetRemoteTransacoes();
            await GetLocalTransacoes();

            IsVisible = !Transacoes.Any();

            MessagingCenter.Send(new RelExtratoMC { Transacoes = Transacoes, Saldo = Saldo }, "RelTransacoes");
        }

        private void ExecuteOpenMenuFiltroCommand()
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
    }
}
