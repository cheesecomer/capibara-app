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

        private string imageBase64;

        public AsyncReactiveCommand CloseCommand { get; }

        public AsyncReactiveCommand SpeakCommand { get; }

        public AsyncReactiveCommand AttachmentImageCommand { get; }

        public AsyncReactiveCommand ShowParticipantsCommand { get; }

        public ReadOnlyReactiveCollection<MessageViewModel> Messages { get; }

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Capacity { get; }

        public ReactiveProperty<int> NumberOfParticipants { get; }

        public ReactiveProperty<bool> IsConnected { get; }

        protected override string OptionalScreenName => $"/{this.Model.Id}";

        public RoomPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Messages = this.Model
                .Messages
                .ToReadOnlyReactiveCollection(x => new MessageViewModel(navigationService, pageDialogService, x).BuildUp(this.Container));

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

            // CloseCommand
            this.CloseCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CloseCommand.Subscribe(async () =>
            {
                this.Model.Disconnected -= this.OnDisconnected;

                if (!this.needClose) return;
                await this.Model.Close();
            });

            // SpeakCommand
            this.SpeakCommand = this.PropertyChangedAsObservable()
                .Select(x => this.Message.Value.ToSlim().IsPresent() && this.IsConnected.Value)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.SpeakCommand.Subscribe(() => this.Model.Speak(this.Message.Value, string.Empty));

            // ShowParticipantsCommand
            this.ShowParticipantsCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowParticipantsCommand.Subscribe(() =>
            {
                this.needClose = false;
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                return this.NavigationService.NavigateAsync("ParticipantsPage", parameters);
            });

            this.Model.SpeakSuccess += (sender, e) =>
            {
                this.Message.Value = string.Empty;
                this.imageBase64 = string.Empty;
            };

            this.Model.SpeakFail += this.OnFail(() => Task.Run(async () =>
            {
                if (this.imageBase64.IsNullOrEmpty())
                {
                    this.SpeakCommand.Execute();
                }
                else
                {
                    await this.ProgressDialogService.DisplayProgressAsync(
                        this.Model.Speak(string.Empty, this.imageBase64));
                }
            }), () => this.imageBase64 = string.Empty);
            
            this.Model.RefreshFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Connect()));

            this.Model.RejectSubscription += async (s, e) =>
            {
                this.Model.Disconnected -= this.OnDisconnected;
                await this.Model.Close();

                this.DeviceService.BeginInvokeOnMainThread(async () =>
                {
                    await this.PageDialogService.DisplayAlertAsync("入室できませんでした", "もう一度やり直すか、時間を置いてお試し下さい", "閉じる");
                    
                    await this.NavigationService.GoBackAsync();
                });
            };

            this.AttachmentImageCommand = new AsyncReactiveCommand();
            this.AttachmentImageCommand.Subscribe(async () => {
                var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });
                var pickupButton = ActionSheetButton.CreateButton("アルバムから選択", async () => {
                    this.needClose = false;
                    var bytes = await this.PickupPhotoService.DisplayAlbumAsync();

                    if (bytes == null) return;

                    this.imageBase64 = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";

                    await this.ProgressDialogService.DisplayProgressAsync(
                        this.Model.Speak(string.Empty, this.imageBase64));
                });

                await this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, pickupButton);
            });


            this.Model.JoinUser += (s, user) => this.BalloonService.DisplayBalloon($"{user.Nickname} さんが入室しました");
            this.Model.LeaveUser += (s, user) => this.BalloonService.DisplayBalloon($"{user.Nickname} さんが退室しました");
        }

        public override void OnResume()
        {
            base.OnResume();

            this.ProgressDialogService.DisplayProgressAsync(this.Connect());
            this.Model.Disconnected += this.OnDisconnected;
        }

        public override void OnSleep()
        {
            base.OnSleep();

            this.Model.Disconnected -= this.OnDisconnected;
        }

        private void OnDisconnected(object sender, EventArgs args)
        {
            this.ProgressDialogService.DisplayProgressAsync(this.Connect());
        }

        private async Task Connect()
        {
            this.needClose = true;
            if (!await this.Model.Refresh()) return;
            await this.Model.Connect();
        }
    }
}
