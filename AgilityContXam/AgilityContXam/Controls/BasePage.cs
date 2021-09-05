using AgilityContXam.ViewModels;
using Xamarin.Forms;

namespace AgilityContXam.Controls
{
    public class BasePage : ContentPage
    {
        protected override bool OnBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;
            var result = bindingContext?.OnBackButtonPressed() ?? base.OnBackButtonPressed();
            return result;
        }

        public void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;
            bindingContext?.OnSoftBackButtonPressed();
        }

        public bool NeedOverrideSoftBackButton { get; set; } = false;
    }
}
