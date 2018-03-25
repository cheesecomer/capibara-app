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
    public class RoomPageViewModel : ViewModelBase<Room>
    {
        private bool needClose = true;

        public AsyncReactiveCommand ConnectCommand { get; }

        public AsyncReactiveCommand CloseCommand { get; }

        public AsyncReactiveCommand SpeakCommand { get; }

        public AsyncReactiveCommand ShowParticipantsCommand { get; }

        public ReadOnlyReactiveCollection<MessageViewModel> Messages { get; }

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Capacity { get; }

        public ReactiveProperty<int> NumberOfParticipants { get; }

        public ReactiveProperty<bool> IsConnected { get; }

        public RoomPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Messages = this.Model
                .Messages
                .ToReadOnlyReactiveCollection(x => new MessageViewModel(navigationService, pageDialogService, x));

            // プロフィールページ表示時にコネクションをクローズしないようにイベント登録
            this.Messages
                .CollectionChangedAsObservable()
                .Subscribe(
                    (x) => x.NewItems
                    .Cast<MessageViewModel>()
                    .ForEach(
                        v => v.ShowProfileCommand
                        .Subscribe(_ => Task.Run(() => this.needClose = false))
                        .AddTo(this.Disposable)))
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

            this.Name.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Name)));
            this.Capacity.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Capacity)));
            this.NumberOfParticipants.Subscribe(_ => this.RaisePropertyChanged(nameof(this.NumberOfParticipants)));
            this.Name.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Name)));
            this.IsConnected.Subscribe(_ => this.RaisePropertyChanged(nameof(this.IsConnected)));
            this.Message.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Message)));

            // ConnectCommand
            this.ConnectCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ConnectCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Connect()));

            // CloseCommand
            this.CloseCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CloseCommand.Subscribe(async () =>
            {
                if (!this.needClose) return;
                await this.Model.Close();
            });

            // SpeakCommand
            this.SpeakCommand = this.PropertyChangedAsObservable()
                .Select(x => this.Message.Value.ToSlim().IsPresent() && this.IsConnected.Value)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.SpeakCommand.Subscribe(() => this.Model.Speak(this.Message.Value));

            // ShowParticipantsCommand
            this.ShowParticipantsCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowParticipantsCommand.Subscribe(() =>
            {
                this.needClose = false;
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                return this.NavigationService.NavigateAsync("ParticipantsPage", parameters);
            });

            this.Model.SpeakSuccess += (sender, e) => this.Message.Value = string.Empty;

            this.Model.SpeakFail += this.OnFail(
                () => Task.Run(
                    () => this.DeviceService.BeginInvokeOnMainThread(
                        () => this.SpeakCommand.Execute())));
            
            this.Model.RefreshFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Connect()));
        }

        private async Task Connect()
        {
            this.needClose = true;
            if (!await this.Model.Refresh()) return;
            await this.Model.Connect();
        }
    }
}
