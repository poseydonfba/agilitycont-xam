using AgilityContXam.Interfaces;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using Refit;
using System;
using System.Diagnostics;

namespace AgilityContXam
{
    public static class AsyncErrorHandler
    {
        public static async void HandleException(Exception exception)
        {
            Debug.WriteLine(exception.Message);

            ApiException refitException = exception as ApiException;
            if (refitException != null)
            {
                if (refitException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //App.Current.MainPage.DisplayAlert("Ops", "", "OK");

                    var _accountService = ((PrismApplication)Xamarin.Forms.Application.Current).Container.Resolve<IAccountService>();
                    var NavigationService = ((PrismApplication)Xamarin.Forms.Application.Current).Container.Resolve<INavigationService>();

                    _accountService.LogoutAsync();
                    await NavigationService.NavigateAsync("/LoginPage");
                }
            }
        }
    }
}
