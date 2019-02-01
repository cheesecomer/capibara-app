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
        private readonly WebNavigatingEventArgs origin;

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
    }
}
