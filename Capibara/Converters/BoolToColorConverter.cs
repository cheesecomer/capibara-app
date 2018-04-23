using System;
using System.Globalization;
using Xamarin.Forms;

namespace Capibara.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public Color TruthyColor { get; set; }

        public Color FalsyColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? this.TruthyColor : this.FalsyColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.TruthyColor == ((Color)value);
        }
    }
}
