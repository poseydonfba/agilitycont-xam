using System;
using System.Globalization;
using Xamarin.Forms;

namespace AgilityContXam.Converters
{
    public class CurrencyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)App.Current.Resources["DarkTextColor"];

            switch (value)
            {
                case 1: // Receita
                    color = (Color)App.Current.Resources["BlueColor"];
                    break;
                case 2:
                    color = (Color)App.Current.Resources["RedColor"];
                    break;
                case 3: // Despesa
                    color = (Color)App.Current.Resources["OrangeColor"];
                    break;
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
