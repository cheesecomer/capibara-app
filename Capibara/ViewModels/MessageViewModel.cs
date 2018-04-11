using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class MessageViewModel : ViewModelBase<Message>
    {
        public ReactiveProperty<int> Id { get; }

        public ReactiveProperty<string> Content { get; }

        public ReactiveProperty<bool> IsOwn { get; }

        public ReactiveProperty<DateTimeOffset> At { get; }

        public ReactiveProperty<UserViewModel> Sender { get; }

        public AsyncReactiveCommand ShowProfileCommand { get; }

        public MessageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Message model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Id = this.Model
                .ObserveProperty(x => x.Id)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Content = this.Model
                .ObserveProperty(x => x.Content)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.At = this.Model
                .ObserveProperty(x => x.At)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Sender = this.Model
                .ObserveProperty(x => x.Sender)
                .Select(x => new UserViewModel(navigationService, pageDialogService, x))
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.IsOwn = this.Model
                .ObserveProperty(x => x.IsOwn)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // ShowProfileCommand
            this.ShowProfileCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowProfileCommand.Subscribe(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, this.Sender.Value.Model);
                return this.NavigationService.NavigateAsync("UserProfilePage", parameters);
            });
        }
    }
}
