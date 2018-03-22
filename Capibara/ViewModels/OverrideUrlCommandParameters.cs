using System;
using System.Linq;

using Prism.Services;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public interface IOverrideUrlCommandParameters
    {
        string Url { get; }

        bool Cancel { get; set; }
    }

    public class OverrideUrlCommandParameters : IOverrideUrlCommandParameters
    {
        private WebNavigatingEventArgs origin;

        string IOverrideUrlCommandParameters.Url => this.origin.Url;

        bool IOverrideUrlCommandParameters.Cancel
        {
            get => this.origin.Cancel;
            set => this.origin.Cancel = value;
        }

        public OverrideUrlCommandParameters(WebNavigatingEventArgs origin)
        {
            this.origin = origin;
        }

        public static Action<IOverrideUrlCommandParameters> OverrideUrl(IDeviceService deviceService, params string[] urls)
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
