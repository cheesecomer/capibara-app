using System;
using System.Globalization;
using Xamarin.Forms;

using Capibara.Services;

using Prism.Services;

namespace Capibara.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TruthyText { get; set; }

        public string FalsyText { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? this.TruthyText : this.FalsyText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.TruthyText == (value as string);
        }
    }
}
