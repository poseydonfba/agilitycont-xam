using AgilityContXam.Enums;
using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Prism.Navigation;
using Prism.Services;
using Refit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace AgilityContXam.ViewModels
{
    public class TransacaoPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly ITransacaoService _transacaoService;

        private const int PageSize = 20;
        private int PageAtual { get; set; }

        public ObservableCollection<Transacao> Transacoes { get; }

        ////public InfiniteScrollCollection<Transacao> Transacoes { get; }
        //private InfiniteScrollCollection<Transacao> _transacoes;
        //public InfiniteScrollCollection<Transacao> Transacoes
        //{
        //    get => _transacoes;
        //    set { _transacoes = value; RaisePropertyChanged(); }
        //}

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; RaisePropertyChanged(); }
        }



        public Command AbrirNovaTransacaoCommand { get; }
        public Command AbrirNovaTransacao2Command { get; }
        public Command SelectedItemCommand { get; }
        public Command InfiniteScrollCommand { get; }

        public TransacaoPageViewModel(INavigationService navigationService,
                                      IPageDialogService dialogService,
                                      ITransacaoService transacaoService)
            : base(navigationService)
        {
            _dialogService = dialogService;
            _transacaoService = transacaoService;

            Title = "Transação";
            IsVisible = false;
            IsNavigate = false;
            PageAtual = 1;

            Transacoes = new ObservableCollection<Transacao>();

            AbrirNovaTransacaoCommand = new Command(ExecuteAbrirNovaTransacaoCommand);
            AbrirNovaTransacao2Command = new Command(ExecuteAbrirNovaTransacao2Command);
            SelectedItemCommand = new Command<Transacao>(ExecuteSelectedItemCommand);
            InfiniteScrollCommand = new Command(ExecuteInfiniteScrollCommand);

            //Transacoes = new InfiniteScrollCollection<Transacao>
            //{
            //    OnLoadMore = async () =>
            //    {
            //        IsBusy = true;
            //        var page = Transacoes.Count / PageSize;
            //        var items = await _transacaoService
            //            .ObterTodosPorUsuarioIdAsync(Settings.Id, page + 1, PageSize, Priority.UserInitiated, true);
            //        await App.SQLiteDb.SaveAllTransacaoAsync(items.ToList());
            //        IsBusy = false;
            //        return items;
            //    }
            //    //OnCanLoadMore = () => Transacoes.Count < TOTAL
            //};

            MessagingCenter.Subscribe<Transacao>(this, "AddNew", (transacao) =>
            {
                Transacoes.Insert(0, transacao);
            });

            MessagingCenter.Subscribe<Transacao>(this, "Update", (transacao) =>
            {
                var tr = Transacoes.FirstOrDefault(d => d.Id == transacao.Id);
                if (tr != null)
                {
                    var index = Transacoes.IndexOf(tr);
                    Transacoes[index] = transacao;
                }
                //var tr = Transacoes.FirstOrDefault(d => d.Id == transacao.Id);
                //if (tr != null)
                //{
                //    tr.IdTipoLancamento = transacao.IdTipoLancamento;
                //    tr.IdTipoTransacao = transacao.IdTipoTransacao;
                //    tr.Descricao = transacao.Descricao;
                //    tr.DataTransacao = transacao.DataTransacao;
                //    tr.Valor = transacao.Valor;
                //    tr.Desconto = transacao.Desconto;
                //    tr.IdFormaPagamento = transacao.IdFormaPagamento;
                //}
            });

            MessagingCenter.Subscribe<Transacao>(this, "Delete", (transacao) =>
            {
                var tr = Transacoes.FirstOrDefault(d => d.Id == transacao.Id);
                if (tr != null)
                    Transacoes.Remove(tr);
            });
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            //await GetLocalTransacoes();
            await GetRemoteTransacoes();
            await GetLocalTransacoes();

            IsVisible = !Transacoes.Any();
        }

        private async void ExecuteInfiniteScrollCommand()
        {
            int page = (Transacoes.Count / PageSize) + 1;
            if (page == PageAtual)
                return;

            //await GetLocalTransacoes();
            await GetRemoteTransacoes();
            await GetLocalTransacoes(false);

            IsVisible = !Transacoes.Any();
        }

        private async void ExecuteSelectedItemCommand(Transacao transacao)
        {
            if (IsNavigate) return;

            IsNavigate = true;

            await NavigationService.NavigateAsync("NovaTransacaoPage",
                ("Transacao", transacao),
                ("DataActionType", DataActionType.Existing));

            IsNavigate = false;
        }

        private async Task GetLocalTransacoes(bool clear = true)
        {
            try
            {
                if (clear)
                    Transacoes.Clear();

                int page = (Transacoes.Count / PageSize) + 1;
                var items = await App.SQLiteDb.Transacao.GetAsync(page, PageSize);

                //items.ForEach(x =>
                //{
                //    x.Valor = x.IdTipoLancamento == 1 ? x.Valor : -1 * x.Valor;
                //});

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
                int page = (Transacoes.Count / PageSize) + 1;
                var items = await _transacaoService
                    .ObterTodosPorUsuarioIdAsync(page, PageSize, Priority.UserInitiated, true);

                if (items != null)
                    await App.SQLiteDb.Transacao.SaveAllAsync(items.ToList());

                IsBusy = false;
            }
            //catch (ApiException ex)
            //{
            //    var statusCode = ex.StatusCode;
            //    var error = ex.GetContentAs<ErrorResponse>();
            //    // deal with error.Message or something
            //}
            catch (Exception ex)
            {
                await _dialogService.DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteAbrirNovaTransacaoCommand()
        {
            await NavigationService.NavigateAsync("NovaTransacaoPage",
                ("DataActionType", DataActionType.New));
        }

        private async void ExecuteAbrirNovaTransacao2Command()
        {
            await NavigationService.NavigateAsync("NovaTransacaoPage",
                ("DataActionType", DataActionType.New));
        }
    }
}
