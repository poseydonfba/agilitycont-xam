using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace AgilityContXam.Converters
{
    public class NoHasDataConverter : IValueConverter
    {
        public static void Init()
        {
            var time = DateTime.UtcNow;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            if (value is string)
                return string.IsNullOrWhiteSpace((string)value);

            if (value is IList)
                return ((IList)value).Count == 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
