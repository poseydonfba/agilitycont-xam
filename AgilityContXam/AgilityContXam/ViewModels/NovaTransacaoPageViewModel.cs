using AgilityContXam.Enums;
using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using Fusillade;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class NovaTransacaoPageViewModel : ViewModelBase, IAutoInitialize
    {
        private readonly IPageDialogService _dialogService;
        private readonly ICentroCustoService _centroCustoService;
        private readonly IFormaPagamentoService _formaPagamentoService;
        private readonly ITipoDespesaService _tipoDespesaService;
        private readonly ITipoReceitaService _tipoReceitaService;
        private readonly ITransacaoService _transacaoService;

        public ObservableCollection<TipoLancamento> TipoLancamentos { get; }
        public ObservableCollection<Item> Items { get; }
        public ObservableCollection<FormaPagamento> FormaPagamentos { get; }
        public ObservableCollection<CentroCusto> CentroCustos { get; }
        public ObservableCollection<TipoDespesa> TipoDespesas { get; }
        public ObservableCollection<TipoReceita> TipoReceitas { get; }

        public Command SalvarTransacaoCommand { get; }
        public Command ExcluirTransacaoCommand { get; }
        public Command CancelarTransacaoCommand { get; }

        public Transacao Transacao { get; set; }
        public DataActionType DataActionType { get; set; }


        private Guid _transacaoId = Guid.NewGuid();
        public Guid TransacaoId
        {
            get => _transacaoId;
            set
            {
                _transacaoId = value;
                RaisePropertyChanged();
            }
        }


        TipoLancamento _selectedTipoLancamento;
        public TipoLancamento SelectedTipoLancamento
        {
            get => _selectedTipoLancamento;
            set
            {
                if (SelectedTipoLancamento != value)
                {
                    _selectedTipoLancamento = value;
                    switch (_selectedTipoLancamento.Id)
                    {
                        case 1:
                            Items.Clear();
                            Items.AddRange(TipoReceitas.Select(x => new Item { Id = x.Id, Descricao = x.Descricao }).OrderBy(x => x.Descricao));
                            break;
                        case 2:
                            Items.Clear();
                            Items.AddRange(CentroCustos.Select(x => new Item { Id = x.Id, Descricao = x.Descricao }).OrderBy(x => x.Descricao));
                            break;
                        case 3:
                            Items.Clear();
                            Items.AddRange(TipoDespesas.Select(x => new Item { Id = x.Id, Descricao = x.Descricao }).OrderBy(x => x.Descricao));
                            break;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SelectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _descricao;
        public string Descricao
        {
            get => _descricao;
            set
            {
                _descricao = value;
                RaisePropertyChanged();
            }
        }

        DateTime _dataTransacao = DateTime.Today;
        public DateTime DataTransacao
        {
            get => _dataTransacao;
            set
            {
                _dataTransacao = value;
                RaisePropertyChanged();
            }
        }

        FormaPagamento _selectedFormaPagamento;
        public FormaPagamento SelectedFormaPagamento
        {
            get => _selectedFormaPagamento;
            set
            {
                if (SelectedFormaPagamento != value)
                {
                    _selectedFormaPagamento = value;
                    RaisePropertyChanged();
                }
            }
        }

        double _valor;
        public double Valor
        {
            get => _valor;
            set
            {
                _valor = value;
                RaisePropertyChanged();
            }
        }

        double _desconto;
        public double Desconto
        {
            get => _desconto;
            set
            {
                _desconto = value;
                RaisePropertyChanged();
            }
        }

        private bool _excluirIsVisible = false;
        public bool ExcluirIsVisible
        {
            get => _excluirIsVisible;
            set { _excluirIsVisible = value; RaisePropertyChanged(); }
        }


        public NovaTransacaoPageViewModel(INavigationService navigationService,
                                          IPageDialogService dialogService,
                                          ICentroCustoService centroCustoService,
                                          IFormaPagamentoService formaPagamentoService,
                                          ITipoDespesaService tipoDespesaService,
                                          ITipoReceitaService tipoReceitaService,
                                          ITransacaoService transacaoService)
            : base(navigationService)
        {
            _dialogService = dialogService;
            _centroCustoService = centroCustoService;
            _formaPagamentoService = formaPagamentoService;
            _tipoDespesaService = tipoDespesaService;
            _tipoReceitaService = tipoReceitaService;
            _transacaoService = transacaoService;

            Title = "Transação";

            TipoLancamentos = new ObservableCollection<TipoLancamento>();
            Items = new ObservableCollection<Item>();
            FormaPagamentos = new ObservableCollection<FormaPagamento>();
            CentroCustos = new ObservableCollection<CentroCusto>();
            TipoDespesas = new ObservableCollection<TipoDespesa>();
            TipoReceitas = new ObservableCollection<TipoReceita>();

            SalvarTransacaoCommand = new Command(ExecuteSalvarTransacaoCommand);
            ExcluirTransacaoCommand = new Command(ExecuteExcluirTransacaoCommand);
            CancelarTransacaoCommand = new Command(ExecuteCancelarTransacaoCommand);
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            GetLocalTipoLancamento();
            await GetLocalFormaPagamento();
            await GetLocalCentroCusto();
            await GetLocalTipoDespesa();
            await GetLocalTipoReceita();

            if (DataActionType == DataActionType.Existing)
            {
                ExcluirIsVisible = true;

                TransacaoId = Transacao.Id;
                SelectedTipoLancamento = TipoLancamentos.FirstOrDefault(x => x.Id == Transacao.IdTipoLancamento);
                SelectedItem = Items.FirstOrDefault(x => x.Id == Transacao.IdTipoTransacao);
                Descricao = Transacao.Descricao;
                DataTransacao = Transacao.DataTransacao;
                Valor = Transacao.Valor;
                Desconto = Transacao.Desconto;
                SelectedFormaPagamento = FormaPagamentos.FirstOrDefault(x => x.Id == Transacao.IdFormaPagamento);
            }
        }

        private void GetLocalTipoLancamento()
        {
            var result = new List<TipoLancamento>
            {
                new TipoLancamento{ Id = 1, Descricao = "Receita" },
                //new TipoLancamento{ Id = 2, Descricao = "Custo" },
                new TipoLancamento{ Id = 3, Descricao = "Despesa" }
            };

            TipoLancamentos.Clear();
            TipoLancamentos.AddRange(result.OrderBy(x => x.Descricao));
        }

        private async Task GetLocalFormaPagamento()
        {
            var result = await App.SQLiteDb.GetFormaPagamentoAsync();

            FormaPagamentos.Clear();
            FormaPagamentos.AddRange(result.OrderBy(x => x.Descricao));
        }

        private async Task GetLocalCentroCusto()
        {
            var result = await App.SQLiteDb.GetCentroCustoAsync();

            CentroCustos.Clear();
            CentroCustos.AddRange(result.OrderBy(x => x.Descricao));
        }

        private async Task GetLocalTipoDespesa()
        {
            var result = await App.SQLiteDb.GetTipoDespesaAsync();

            TipoDespesas.Clear();
            TipoDespesas.AddRange(result.OrderBy(x => x.Descricao));
        }

        private async Task GetLocalTipoReceita()
        {
            var result = await App.SQLiteDb.GetTipoReceitaAsync();

            TipoReceitas.Clear();
            TipoReceitas.AddRange(result.OrderBy(x => x.Descricao));
        }

        public async void ExecuteSalvarTransacaoCommand()
        {
            if (IsBusy)
                return;

            if (SelectedTipoLancamento == null)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Selecione um Tipo de Lançamento", "OK");
                return;
            }
            if (SelectedItem == null)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Selecione um Tipo", "OK");
                return;
            }
            if (string.IsNullOrEmpty(Descricao))
            {
                await _dialogService.DisplayAlertAsync("Ops", "Digite uma Descrição", "OK");
                return;
            }
            if (DataTransacao == null)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Selecione a Data da Transação", "OK");
                return;
            }
            if (SelectedFormaPagamento == null)
            {
                await _dialogService.DisplayAlertAsync("Ops", "Selecione a Forma de Pagamento", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                if (DataActionType == DataActionType.Existing)
                {
                    var model = new AlterarTransacaoBindingModel
                    {
                        Id = TransacaoId,
                        IdTipoLancamento = SelectedTipoLancamento.Id,
                        IdTipoTransacao = SelectedItem.Id,
                        Descricao = Descricao,
                        DataTransacao = DataTransacao,
                        Valor = Valor,
                        Desconto = Desconto,
                        IdFormaPagamento = SelectedFormaPagamento.Id
                    };
                    await _transacaoService.AlterarAsync(model);

                    var transacao = new Transacao
                    {
                        Id = model.Id,
                        IdTipoLancamento = model.IdTipoLancamento,
                        IdTipoTransacao = model.IdTipoTransacao,
                        Descricao = model.Descricao,
                        DataTransacao = model.DataTransacao,
                        Valor = model.Valor,
                        Desconto = model.Desconto,
                        IdFormaPagamento = model.IdFormaPagamento,
                        DescFormaPagamento = SelectedFormaPagamento.Descricao,
                        DescTipoLancamento = SelectedTipoLancamento.Descricao,
                        DescTipoTransacao = SelectedItem.Descricao
                    };
                    await App.SQLiteDb.Transacao.SaveAsync(transacao);

                    MessagingCenter.Send(transacao, "Update");
                }
                else
                {
                    var model = new IncluirTransacaoBindingModel
                    {
                        Id = TransacaoId,
                        IdTipoLancamento = SelectedTipoLancamento.Id,
                        IdTipoTransacao = SelectedItem.Id,
                        Descricao = Descricao,
                        DataTransacao = DataTransacao,
                        Valor = Valor,
                        Desconto = Desconto,
                        IdFormaPagamento = SelectedFormaPagamento.Id
                    };
                    await _transacaoService.InserirAsync(model);

                    var transacao = new Transacao
                    {
                        Id = model.Id,
                        IdTipoLancamento = model.IdTipoLancamento,
                        IdTipoTransacao = model.IdTipoTransacao,
                        Descricao = model.Descricao,
                        DataTransacao = model.DataTransacao,
                        Valor = model.Valor,
                        Desconto = model.Desconto,
                        IdFormaPagamento = model.IdFormaPagamento,
                        DescFormaPagamento = SelectedFormaPagamento.Descricao,
                        DescTipoLancamento = SelectedTipoLancamento.Descricao,
                        DescTipoTransacao = SelectedItem.Descricao
                    };
                    await App.SQLiteDb.Transacao.SaveAsync(transacao);

                    MessagingCenter.Send(transacao, "AddNew");
                }

                IsBusy = false;

                await _dialogService.DisplayAlertAsync("Sucesso", "Sucesso na operação", "OK");

                await NavigationService.GoBackAsync();
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

        public async void ExecuteExcluirTransacaoCommand()
        {
            if (IsBusy)
                return;

            try
            {
                var result = await _dialogService.DisplayAlertAsync("Atenção!", "Deseja excluir esta transação?", "Sim", "Não");
                if (result)
                {
                    IsBusy = true;

                    if (DataActionType == DataActionType.Existing)
                    {
                        var model = new ExcluirTransacaoBindingModel
                        {
                            Id = TransacaoId
                        };
                        await _transacaoService.ExcluirAsync(model);

                        await App.SQLiteDb.Transacao.DeleteAsync(Transacao);

                        IsBusy = false;

                        await _dialogService.DisplayAlertAsync("Sucesso", "Sucesso na operação", "OK");

                        MessagingCenter.Send(new Transacao { Id = TransacaoId }, "Delete");
                    }

                    await NavigationService.GoBackAsync();
                }
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

        public async void ExecuteCancelarTransacaoCommand()
        {
            await NavigationService.GoBackAsync();
        }
    }
}
