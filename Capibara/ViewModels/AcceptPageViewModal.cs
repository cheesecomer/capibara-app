using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Capibara.Models;
using Capibara.Services;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class AcceptPageViewModel : ViewModelBase<User>
    {
        public Task NextCommandTask { get; private set; }

        [Unity.Dependency]
        public IOverrideUrlService OverrideUrlService { get; set; }

        public ReactiveProperty<bool> IsLoaded { get; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<UrlWebViewSource> Source { get; } = new ReactiveProperty<UrlWebViewSource>();

        public ReactiveProperty<string> ActiveCommandName { get; }

        public ReactiveProperty<string> Title { get; }

        public ReactiveProperty<ICommand> ActiveCommand { get; } = new ReactiveProperty<ICommand>();

        public AsyncReactiveCommand AgreeCommand { get; }

        public AsyncReactiveCommand NextCommand { get; }

        public AsyncReactiveCommand CancelCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand LoadedCommand { get; } = new ReactiveCommand();

        public ReactiveCommand<IOverrideUrlCommandParameters> OverrideUrlCommand { get; } =
            new ReactiveCommand<IOverrideUrlCommandParameters>();

        public AcceptPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.IsLoaded.Subscribe(_ => this.RaisePropertyChanged(nameof(IsLoaded)));
            this.Source.Subscribe(_ => this.RaisePropertyChanged(nameof(Source)));

            this.NextCommand = this.PropertyChangedAsObservable()
                .Select(_ => this.IsLoaded.Value && this.Source.Value.Url == this.Environment.TermsUrl)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.NextCommand.Subscribe(() => this.NextCommandTask = Task.Run(() =>
            {
                this.IsLoaded.Value = false;
                this.Source.Value = new UrlWebViewSource { Url = this.Environment.PrivacyPolicyUrl };
                this.ActiveCommand.Value = this.AgreeCommand;
            }));

            this.AgreeCommand = this.PropertyChangedAsObservable()
                .Select(_ => this.IsLoaded.Value && this.Source.Value.Url == this.Environment.PrivacyPolicyUrl)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.AgreeCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Accept()));

            this.CancelCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Destroy()));

            this.LoadedCommand.Subscribe(() => this.IsLoaded.Value = true);

            this.Model.AcceptSuccess += this.OnAcceptSuccess;
            this.Model.AcceptFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Accept()));

            this.Model.DestroySuccess += this.OnDestroySuccess;
            this.Model.DestroyFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Destroy()));

            this.ActiveCommand.Value = this.NextCommand;
            this.ActiveCommandName = this.ActiveCommand
                .Select(x => x == this.NextCommand ? "次へ" : "同意する")
                .ToReactiveProperty();
            this.Title = this.ActiveCommand
                .Select(x => x == this.NextCommand ? "利用規約の同意" : "プライバシーポリシーの同意")
                .ToReactiveProperty();
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.Source.Value = new UrlWebViewSource { Url = this.Environment.TermsUrl };

            this.OverrideUrlCommand.Subscribe(
                this.OverrideUrlService.OverrideUrl(
                    this.DeviceService,
                    this.Environment.TermsUrl,
                    this.Environment.PrivacyPolicyUrl));
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            this.Model.IsAccepted = true;
        }

        private async void OnAcceptSuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/NavigationPage/MainPage");
        }

        private async void OnDestroySuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/SignUpPage");
        }
    }
}
