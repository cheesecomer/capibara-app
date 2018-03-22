using System;
using System.Linq;

using Capibara.ViewModels;

using Prism.Services;

namespace Capibara.Services
{
    public interface IOverrideUrlService
    {
        Action<IOverrideUrlCommandParameters> OverrideUrl(IDeviceService deviceService, params string[] urls);
    }

    public class OverrideUrlService : IOverrideUrlService
    {
        Action<IOverrideUrlCommandParameters> IOverrideUrlService.OverrideUrl(IDeviceService deviceService, params string[] urls)
        {
            return (x) =>
            {
                if (urls.Any(v => new Uri(x.Url).ToString() == new Uri(v).ToString())) return;

                x.Cancel = true;

                deviceService.OpenUri(new Uri(x.Url));
            };
        }
    }
}
