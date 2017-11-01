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
    public class SignUpPageViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public AsyncReactiveCommand SignInCommand { get; }

        public AsyncReactiveCommand SignUpCommand { get; }

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
            this.SignUpCommand = this.PropertyChangedAsObservable()
                .Select(_ => this.Nickname.Value.ToSlim().IsPresent())
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
        }

        private void OnSignUpSuccess(object sender, EventArgs args)
        {
            this.NavigationService.NavigateAsync("/MainPage/NavigationPage/FloorMapPage");
        }
    }
}
