using System;
using System.Globalization;
using Capibara.ViewModels;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class WebNavigatingEventArgsConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new OverrideUrlCommandParameters(value as WebNavigatingEventArgs) as IOverrideUrlCommandParameters;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
