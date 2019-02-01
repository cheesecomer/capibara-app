using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Capibara.Attributes;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class EnumToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (value as IEnumerable)?.Cast<Enum>()?.Select(this.DisplayName)?.ToList() ?? value;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private string DisplayName(Enum value)
        => value
            .GetType()
            .GetRuntimeField(value.ToString())
            ?.GetCustomAttributes(typeof(DisplayNameAttribute), false)
            ?.Select(v => v as DisplayNameAttribute)
            ?.Where(v => v.IsPresent())
            ?.FirstOrDefault()
            ?.DisplayName ?? value.ToString();
    }
}
