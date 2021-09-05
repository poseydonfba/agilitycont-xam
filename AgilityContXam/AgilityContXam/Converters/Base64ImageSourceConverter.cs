using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace AgilityContXam.Converters
{
    public class Base64ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource retSource = null;

            if (value != null && (string)value != "")
            {
                var base64Image = (string)value;

                retSource = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(System.Convert.FromBase64String(base64Image));
                });
            }
            else
            {
                var b64 = "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";
                
                retSource = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(System.Convert.FromBase64String(b64));
                });
            }

            return retSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
