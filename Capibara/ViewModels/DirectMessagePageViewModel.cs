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
    public class DirectMessagePageViewModel : ViewModelBase<DirectMessageThread>
    {
        private bool needClose = true;

        public AsyncReactiveCommand CloseCommand { get; }

        public AsyncReactiveCommand SpeakCommand { get; }

        public ReadOnlyReactiveCollection<DirectMessageViewModel> Messages { get; }

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<bool> IsConnected { get; }

        protected override string OptionalScreenName => $"/{this.Model.User.Id}";

        public DirectMessagePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            DirectMessageThread model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Messages = this.Model
                .DirectMessages
                .ToReadOnlyReactiveCollection(x => new DirectMessageViewModel(navigationService, pageDialogService, x).BuildUp(this.Container));

            // プロフィールページ表示時にコネクションをクローズしないようにイベント登録
            this.Messages
                .CollectionChangedAsObservable()
                .Subscribe(
                    (x) => x.NewItems
                    ?.Cast<DirectMessageViewModel>()
                    .ForEach(
                        v =>
                        {
                            v.ShowProfileCommand.Subscribe(_ => Task.Run(() => this.needClose = false)).AddTo(this.Disposable);
                        }))
                .AddTo(this.Disposable);

            // Name Property
            this.Name = this.Model.User
                .ObserveProperty(x => x.Nickname)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // IsConnect Property
            this.IsConnected = this.Model
                .ObserveProperty(x => x.IsConnected)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

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
            this.SpeakCommand.Subscribe(() => this.Model.Speak(this.Message.Value));

            this.Model.SpeakSuccess += (sender, e) =>
            {
                this.Message.Value = string.Empty;
            };

            this.Model.SpeakFail += this.OnFail(() => this.Model.Speak(this.Message.Value));

            this.Model.RefreshFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Connect()));
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
