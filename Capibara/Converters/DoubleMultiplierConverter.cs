using System;
using System.Globalization;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class DoubleMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double multiplier;

            if (!Double.TryParse(parameter as string, out multiplier))
                multiplier = 1;
            
            if (value is float)
            {
                return multiplier * (double)(float)value;
            }
            else if (value is double)
            {
                return multiplier * (double)value;
            }
            else if (value is int)
            {
                return multiplier * (double)(int)value;
            }
            else
            {
                return -1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
