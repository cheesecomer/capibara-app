﻿using System;
using System.Globalization;
using Xamarin.Forms;

using Capibara.Services;

using Prism.Services;

namespace Capibara.Converters
{
    public class TopMarginConverter: IValueConverter
    {
        private IDeviceService deviceService;

        private IScreenService screenService;

        public TopMarginConverter(IDeviceService deviceService, IScreenService screenService)
        {
            this.deviceService = deviceService;
            this.screenService = screenService;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.deviceService.DeviceRuntimePlatform == Device.Android)
            {
                return 0;
            }

            return this.screenService.Size.Height.Equals(2436) ? 44 : 20;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
