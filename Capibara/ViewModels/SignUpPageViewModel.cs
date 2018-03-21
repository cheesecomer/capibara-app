using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Capibara.Models;

using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Unity;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase<User>, IApplicationLifecycleAware
    {
        public ReactiveProperty<string> Nickname { get; }

        public AsyncReactiveCommand SignInCommand { get; }

        public AsyncReactiveCommand SignUpCommand { get; }

        public AsyncReactiveCommand SignUpWithSnsCommand { get; }

        public ReadOnlyReactiveProperty<bool> IsBusy { get; }

        public SignUpPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Model.SignUpSuccess += this.OnSignUpSuccess;

            // Nickname Property
            this.Nickname = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Nickname)
                .AddTo(this.Disposable);
            this.Nickname.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Nickname)));

            // SignUp Command
            this.SignUpCommand = this.Nickname.Select(x => x.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.SignUpCommand.Subscribe(this.Model.SignUp);

            // IsBusy Command
            this.IsBusy = this.SignUpCommand
                .CanExecuteChangedAsObservable()
                // ニックネームが入力された状態で、サインインコマンド実行不可は、現在処理中
                .Select(_ => this.Nickname.Value.ToSlim().IsPresent() && !this.SignUpCommand.CanExecute())
                .ToReadOnlyReactiveProperty()
                .AddTo(this.Disposable);

            // SignIn Command
            this.SignInCommand = this.IsBusy.Select(x => !x).ToAsyncReactiveCommand();
            this.SignInCommand.Subscribe(() => this.NavigationService.NavigateAsync("SignInPage", animated: false));

            // SignUpWithSnsCommand
            this.SignUpWithSnsCommand = this.IsBusy.Select(x => !x).ToAsyncReactiveCommand();
            this.SignUpWithSnsCommand.Subscribe(async () => {
                var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });
                var twitterButton = ActionSheetButton.CreateButton("Twitter", () => this.OpenOAuthUri(OAuthProvider.Twitter));
                var lineButton = ActionSheetButton.CreateButton("LINE", () => this.OpenOAuthUri(OAuthProvider.Line));
                await this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", cancelButton, twitterButton, lineButton);
            });
        }

        public void OnResume()
        {
            if (this.IsolatedStorage.AccessToken.IsPresent())
                this.ProgressDialogService.DisplayProgressAsync(this.SignIn());
        }

        void IApplicationLifecycleAware.OnSleep() { }

        private void OnSignUpSuccess(object sender, EventArgs args)
        {
            var pageName =
                this.Model.IsAccepted
                    ? "/MainPage/NavigationPage/FloorMapPage"
                    : "/AcceptPage";
            var parameters =
                this.Model.IsAccepted 
                    ? null 
                    : new NavigationParameters { { ParameterNames.Model, this.Model } };
            this.NavigationService.NavigateAsync(pageName, parameters);
        }

        private void OpenOAuthUri(OAuthProvider provider)
        {
            var uri = new Uri(Path.Combine(this.Environment.OAuthBaseUrl, provider.ToString().ToLower()));
            this.DeviceService.OpenUri(uri);
        }

        private async Task SignIn()
        {
            var user = new User { Id = this.IsolatedStorage.UserId }.BuildUp(this.Container);
            if (await user.Refresh())
            {
                var pageName = 
                    user.IsAccepted 
                        ? "/MainPage/NavigationPage/FloorMapPage" 
                        : "/AcceptPage";
                var parameters =
                    this.Model.IsAccepted
                        ? null
                        : new NavigationParameters { { ParameterNames.Model, user } };
                await this.NavigationService.NavigateAsync(pageName, parameters);
            }
        }
    }
}
