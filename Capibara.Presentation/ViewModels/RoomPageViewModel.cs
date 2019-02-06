using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Capibara.Domain.Models;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class RoomPageViewModel : ViewModelBase<Room>
    {
        public RoomPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Messages = this.Model.Messages
                .ToReadOnlyReactiveCollection(x => new MessageViewModel(navigationService, pageDialogService, x));

            // Name Property
            this.Name = this.Model
                .ObserveProperty(x => x.Name)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // Capacity Property
            this.Capacity = this.Model
                .ObserveProperty(x => x.Capacity)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // NumberOfParticipants Property
            this.NumberOfParticipants = this.Model
                .ObserveProperty(x => x.NumberOfParticipants)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // IsConnect Property
            this.IsConnected = this.Model
                .ObserveProperty(x => x.IsConnected)
                .ToReactiveProperty()
                .AddTo(this.Disposable);
        }

        public ReadOnlyReactiveCollection<MessageViewModel> Messages { get; }

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Capacity { get; }

        public ReactiveProperty<int> NumberOfParticipants { get; }

        public ReactiveProperty<bool> IsConnected { get; }

    }
}
