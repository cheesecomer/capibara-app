using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class DirectMessageThreadViewModel : ViewModelBase<DirectMessageThread>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<DateTimeOffset> At { get; }
        
        public ReactiveProperty<string> Content { get; }

        public ReactiveProperty<ImageSource> IconThumbnail { get; } = new ReactiveProperty<ImageSource>();

        public AsyncReactiveCommand ShowProfileCommand { get; }

        public DirectMessageThreadViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            DirectMessageThread model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Nickname = this.Model.User
                .ObserveProperty(x => x.Nickname)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Content = this.Model.LatestDirectMessage
                .ObserveProperty(x => x.Content)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.At = this.Model.LatestDirectMessage
                .ObserveProperty(x => x.At)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Model.User
                .ObserveProperty(x => x.IconThumbnailUrl)
                .Subscribe(x => this.IconThumbnail.Value = x);
            
            // ShowProfileCommand
            this.ShowProfileCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowProfileCommand.Subscribe(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, this.Model.User);
                return this.NavigationService.NavigateAsync("UserProfilePage", parameters);
            });
        }
    }
}
