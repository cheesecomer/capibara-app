using System;
using Capibara.Services;

namespace Capibara.Droid.Services
{
    public class SnsLoginService : ISnsLoginService
    {
        void ISnsLoginService.Open(string url) => Xamarin.Forms.Device.OpenUri(new Uri(url));
    }
}
