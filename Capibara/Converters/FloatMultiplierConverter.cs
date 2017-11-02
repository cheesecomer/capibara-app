using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class FloatMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float multiplier;

            if (!float.TryParse(parameter as string, out multiplier))
                multiplier = 1;

            if (value is float)
            {
                return new[] { multiplier * (float)value, 0 }.Max();
            }
            else if (value is double)
            {
                return new[] { multiplier * (float)(double)value, 0 }.Max();
            }
            else if (value is int)
            {
                return new[] { multiplier * (float)(int)value, 0 }.Max();
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
