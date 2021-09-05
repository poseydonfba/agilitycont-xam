using AgilityContXam.Interfaces;
using AgilityContXam.Models;
using PdfSharp.Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AgilityContXam.Views
{
    public partial class RelSaldoPage
    {
        public bool IsLoading { get; set; } = false;

        public RelSaldoPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<RelExtratoMC>(this, "RelTransacoes", (model) =>
            {
                if (IsLoading)
                    return;

                IsLoading = true;

                activityIndicator.IsRunning = IsLoading;

                ObservableCollection<Transacao> transacoes = model.Transacoes;

                mainGrid.Children.Clear();

                var slayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label {
                            Text = "Extrato",
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            Margin = new Thickness(10, 5, 10, 5),
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            FontFamily = Application.Current.Resources["FontNunitoBold"] as string
                        },
                        new Label {
                            Text = "Saldo: " + model.Saldo.ToString("N2"),
                            HorizontalOptions = LayoutOptions.End,
                            Margin = new Thickness(10, 5, 10, 5),
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            FontFamily = Application.Current.Resources["FontNunitoBold"] as string
                        },
                        new BoxView
                        {
                            HeightRequest = 1,
                            BackgroundColor = Color.FromHex("#eeeeee")
                        }
                    }
                };

                mainGrid.Children.Add(slayout);

                foreach (var transacao in transacoes)
                {
                    var grid = new Grid
                    {
                        Margin = new Thickness(10, 0, 10, 0),
                        ColumnDefinitions = {
                            new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength (1, GridUnitType.Auto) },
                            new ColumnDefinition { Width = new GridLength (1, GridUnitType.Auto) }
                        }
                    };
                    grid.Children.Add(new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Start,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        Text = transacao.DescTipoTransacao,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                    }, 0, 0);
                    grid.Children.Add(new Label
                    {
                        HorizontalTextAlignment = TextAlignment.End,
                        HorizontalOptions = LayoutOptions.End,
                        Text = transacao.DataTransacao.ToString("dd/MM/yyyy"),
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                    }, 1, 0);
                    grid.Children.Add(new Label
                    {
                        HorizontalTextAlignment = TextAlignment.End,
                        HorizontalOptions = LayoutOptions.End,
                        Text = transacao.Valor.ToString("N2"),
                        TextColor = transacao.IdTipoLancamento == 1 ? (Color)App.Current.Resources["BlueColor"] :
                                    transacao.IdTipoLancamento == 3 ? (Color)App.Current.Resources["OrangeColor"] :
                                                                      (Color)App.Current.Resources["DarkTextColor"],
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        FontFamily = Application.Current.Resources["FontNunitoBold"] as string
                    }, 2, 0);

                    mainGrid.Children.Add(grid);
                }

                IsLoading = false;

                activityIndicator.IsRunning = IsLoading;
            });
        }

        private async void GeneratePDF(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            IsLoading = true;

            activityIndicator.IsRunning = IsLoading;

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    // Verifica se o usuario ja negou a permissão uma vez
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert("Ops", "Precisamos desta permissão para salvar o PDF no dispositivo.", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    var pdf = PDFManager.GeneratePDFFromView(mainGrid);
                    DependencyService.Get<IPdfSave>().Save(pdf, $"extrato_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.pdf");
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Ops", "Não pode continuar, tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

            IsLoading = false;

            activityIndicator.IsRunning = IsLoading;
        }
    }
}
