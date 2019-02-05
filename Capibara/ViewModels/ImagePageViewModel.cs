using System;

using Capibara.Services;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class ImagePageViewModel : ViewModelBase
    {
        public ReactiveProperty<ImageSource> Image { get; } = new ReactiveProperty<ImageSource>();

        public ImagePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService) { }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var url = parameters.TryGetValue<string>(ParameterNames.Url);
            if (url.IsPresent())
            {
                this.Image.Value = url;
            }
        }
    }
}
