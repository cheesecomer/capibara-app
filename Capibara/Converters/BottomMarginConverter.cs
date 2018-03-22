using System;
using System.Globalization;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class BottomMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return 0;
            }

            var size = DependencyService.Get<IScreen>().Size;

            return size.Height.Equals(2436) ? 34 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
