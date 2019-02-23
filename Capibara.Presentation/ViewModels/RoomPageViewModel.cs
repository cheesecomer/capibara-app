using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
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
            this.Message = new ReactiveProperty<string>()
                .AddTo(this.Disposable);

            this.Messages = this.Model
                .Messages
                .ToReadOnlyReactiveCollection(
                    x => new MessageViewModel(
                    navigationService, 
                    pageDialogService, 
                    x))
                .AddTo(this.Disposable);

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

            this.SpeekCommand = this.Message
                .Select(x => x.ToSlim().IsPresent())
                .CombineLatest(this.IsConnected, (x, y) => x && y)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.SpeekCommand.Subscribe(this.SpeakAsync);

            this.AttachmentImageCommand = this.IsConnected
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.AttachmentImageCommand.Subscribe(this.AttachmentImageAsync);
        }

        public IAttachmentImageUseCase AttachmentImageUseCase { get; set; }

        public IPickupPhotoFromAlbumUseCase PickupPhotoFromAlbumUseCase { get; set; }

        public ISpeekUseCase SpeekUseCase { get; set; }

        public AsyncReactiveCommand AttachmentImageCommand { get; }

        public AsyncReactiveCommand SpeekCommand { get; }

        public ReadOnlyReactiveCollection<MessageViewModel> Messages { get; }

        public ReactiveProperty<string> Message { get; }

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Capacity { get; }

        public ReactiveProperty<int> NumberOfParticipants { get; }

        public ReactiveProperty<bool> IsConnected { get; }

        private Task SpeakAsync()
        {
            return Observable
                .FromAsync(
                    () => this.SpeekUseCase.Invoke(this.Message.Value), 
                    this.SchedulerProvider.IO)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(_ => { }, () => this.Message.Value = string.Empty)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task AttachmentImageAsync()
        {
            return Observable
                .FromAsync(
                    () => this.PickupPhotoFromAlbumUseCase.Invoke(),
                    this.SchedulerProvider.UI)
                .SelectMany(base64 => 
                    Observable.FromAsync(() => this.AttachmentImageUseCase.Invoke(base64))
                        .SubscribeOn(this.SchedulerProvider.IO)
                        .ObserveOn(this.SchedulerProvider.UI)
                        .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI))
                .Select(_ => Unit.Default)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }
    }
}
