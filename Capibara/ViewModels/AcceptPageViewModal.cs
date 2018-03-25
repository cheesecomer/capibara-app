using System;
using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

using Capibara.Services;
using Capibara.Models;

namespace Capibara.ViewModels
{
    public class AcceptPageViewModel : ViewModelBase<User>
    {
        [Unity.Attributes.Dependency]
        public IOverrideUrlService OverrideUrlService { get; set; }

        public ReactiveProperty<bool> IsLoaded { get; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<UrlWebViewSource> Source { get; } = new ReactiveProperty<UrlWebViewSource>();

        public AsyncReactiveCommand AgreeCommand { get; }

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
            this.AgreeCommand = this.IsLoaded.ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.AgreeCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit()));

            this.CancelCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Destroy()));

            this.LoadedCommand.Subscribe(() => this.IsLoaded.Value = true);

            this.Model.CommitSuccess += this.OnCommitSuccess;
            this.Model.CommitFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit()));

            this.Model.DestroySuccess += this.OnDestroySuccess;
            this.Model.DestroyFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Destroy()));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.Source.Value = new UrlWebViewSource { Url = this.Environment.PrivacyPolicyUrl };

            this.OverrideUrlCommand.Subscribe(
                this.OverrideUrlService.OverrideUrl(
                    this.DeviceService,
                    this.Environment.PrivacyPolicyUrl));
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            this.Model.IsAccepted = true;
        }

        private async void OnCommitSuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/MainPage/NavigationPage/FloorMapPage");
        }

        private async void OnDestroySuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/SignUpPage");
        }
    }
}
