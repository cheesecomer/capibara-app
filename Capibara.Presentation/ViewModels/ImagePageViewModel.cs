using Capibara.Presentation.Navigation;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;

namespace Capibara.Presentation.ViewModels
{
    public class ImagePageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> ImageUrl { get; } = new ReactiveProperty<string>();

        public ImagePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService) { }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            var url = parameters.TryGetValue<string>(ParameterNames.Url);
            if (url.IsPresent())
            {
                this.ImageUrl.Value = url;
            }
        }
    }
}
