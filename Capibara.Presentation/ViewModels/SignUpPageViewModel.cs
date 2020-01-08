using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.UseCases;
using Capibara.Domain.Models;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
        public SignUpPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.Nickname.AddTo(this.Disposable);

            // SignUp Command
            this.SignUpCommand = this.Nickname
                .Select(x => x.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.SignUpCommand.Subscribe(SignUpAsync);

            var actionSheetButtons = new[] {
                ActionSheetButton.CreateCancelButton("キャンセル", () => { }),
                ActionSheetButton.CreateButton("Google", () => this.OAuthSignInUseCase.Invoke(OAuthProvider.Google).Wait()),
                ActionSheetButton.CreateButton("Twitter", () => this.OAuthSignInUseCase.Invoke(OAuthProvider.Twitter).Wait()),
                ActionSheetButton.CreateButton("LINE", () => this.OAuthSignInUseCase.Invoke(OAuthProvider.Line).Wait())
            };

            // SignUpByOAuthCommand
            this.SignUpByOAuthCommand = this.SignUpCommand
                .CanExecuteChangedAsObservable()
                .Select(x => this.SignUpCommand.CanExecute() && this.Nickname.Value.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.SignUpByOAuthCommand.Subscribe(
                () => this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", actionSheetButtons));

            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(this.RefreshAsync);
        }

        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>(string.Empty);

        public AsyncReactiveCommand SignUpCommand { get; }

        public AsyncReactiveCommand SignUpByOAuthCommand { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        [Unity.Dependency]
        public ISignUpUseCase SignUpUseCase { get; set; }

        [Unity.Dependency]
        public IOAuthSignInUseCase OAuthSignInUseCase { get; set; }

        [Unity.Dependency]
        public IRefreshSessionUseCase RefreshSessionUseCase { get; set; }

        [Unity.Dependency]
        public IHasSessionUseCase HasSessionUseCase { get; set; }

        protected Task RefreshAsync()
        {
            return this.HasSessionUseCase
                .Invoke()
                .Where(v => v)
                .FirstAsync()
                .SelectMany(_ => this.RefreshSessionUseCase.Invoke())
                .SubscribeOn(this.SchedulerProvider.IO)
                .ObserveOn(this.SchedulerProvider.UI)
                .SelectMany(user =>
                {
                    var pageName =
                        user.IsAccepted
                            ? "/NavigationPage/MainPage"
                            : "/NavigationPage/AcceptPage";

                    var parameters =
                        user.IsAccepted
                            ? null
                            : new NavigationParameters { { ParameterNames.Model, user } };

                    return this.NavigationService
                        .NavigateAsync(pageName, parameters, animated: false)
                        .ToObservable();
                })
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task SignUpAsync()
        {
            return Observable
                .Defer(() => SignUpUseCase.Invoke(this.Nickname.Value))
                .SubscribeOn(this.SchedulerProvider.IO)
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }
    }
}
