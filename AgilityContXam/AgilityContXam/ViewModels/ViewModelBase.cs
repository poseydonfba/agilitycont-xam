using AgilityContXam.Helpers;
using AgilityContXam.Interfaces;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace AgilityContXam.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IMessage MessageService { get; private set; }

        public bool IsConnected { get; set; }
        private IConnectivity connectivity = CrossConnectivity.Current;

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isNavigate = false;
        public bool IsNavigate
        {
            get => _isNavigate;
            set => SetProperty(ref _isNavigate, value);
        }

        #region MENU BOTTOM

        private bool _isSlide = false;
        public bool IsSlide
        {
            get => _isSlide;
            set => SetProperty(ref _isSlide, value);
        }

        private double _defaultHeight;
        public double DefaultHeight
        {
            get => _defaultHeight;
            set => SetProperty(ref _defaultHeight, value);
        }

        private double _defaultWidth;
        public double DefaultWidth
        {
            get => _defaultWidth;
            set => SetProperty(ref _defaultWidth, value);
        }

        #endregion

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            MessageService = DependencyService.Get<IMessage>();

            DefaultHeight = App.Height;
            DefaultWidth = App.Width;

            IsConnected = connectivity.IsConnected;
            connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.IsConnected;

            if (!IsConnected)
            {
                var stringResponse = "Sem conexão"; // Ocorreu um problema com sua conexão de rede.
                MessageService.LongAlert(stringResponse);
            }
        }

        public bool CheckIfJwtIsEmpty()
        {
            return Settings.AccessToken == "";
        }

        public bool CheckIfJwtIsExpired()
        {
            return DateTime.Now > Settings.AccessTokenExpirationDate;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        /// <summary>
        /// //false is default value when system call back press
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBackButtonPressed()
        {
            //false is default value when system call back press
            return false;
        }

        /// <summary>
        /// called when page need override soft back button
        /// </summary>
        public virtual void OnSoftBackButtonPressed() { }
    }
}
