﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

using System.Linq;

using Capibara.Attributes;

namespace Capibara.Converters
{
    public class EnumToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Enumerable.Cast<Enum>(value as IEnumerable)?.Select(this.DisplayName).ToList() ?? value;
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
