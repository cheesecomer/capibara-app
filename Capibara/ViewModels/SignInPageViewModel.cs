using System;
using System.Linq;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class SignInPageViewModel : ViewModelBase<Session>
    {
        public ReactiveProperty<string> Email { get; }

        public ReactiveProperty<string> Password { get; }

        public ReactiveProperty<string> Error { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactiveProperty<bool> IsBusy { get; }

        public AsyncReactiveCommand SignInCommand { get; }

        public AsyncReactiveCommand SignUpCommand { get; }

        public SignInPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Session model = null)
            : base(navigationService, pageDialogService, model)
        {
            // Email property
            this.Email = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Email)
                .AddTo(this.Disposable);
            this.Email.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Email)));

            // Password property
            this.Password = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Password)
                .AddTo(this.Disposable);
            this.Password.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Password)));

            // Error property
            this.Error.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Error)));

            // SignIn Command
            this.SignInCommand = this.PropertyChangedAsObservable()
                .Select(_ => this.Email.Value.IsPresent() && this.Password.Value.IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.SignInCommand.Subscribe(this.Model.SignIn);

            // Event Listener
            this.Model.SignInSuccess += this.OnSignInSuccess;
            this.Model.SignInFail += this.OnSignInFail;

            // IsBusy Command
            this.IsBusy = this.SignInCommand
                .CanExecuteChangedAsObservable()
                // メールアドレスとパスワードが入力された状態で、ログインコマンド実行不可は、現在処理中
                .Select(_ => this.Email.Value.IsPresent() && this.Password.Value.IsPresent() && !this.SignInCommand.CanExecute())
                .ToReadOnlyReactiveProperty()
                .AddTo(this.Disposable);

            // SignUpCommand
            //   ログイン処理中以外で実行可能
            this.SignUpCommand = this.IsBusy.Select(x => !x).ToAsyncReactiveCommand();
            this.SignUpCommand.Subscribe(() => this.NavigationService.NavigateAsync("SignUpPage", animated: false));
        }

        private void OnSignInSuccess(object sender, EventArgs args)
        {
            this.NavigationService.NavigateAsync("/MainPage/NavigationPage/FloorMapPage");
        }

        private void OnSignInFail(object sender, Exception args)
        {
            if (args is Net.HttpUnauthorizedException)
            {
                this.Error.Value = ((Net.HttpUnauthorizedException)args).Detail.Message;
            }
        }
    }
}
