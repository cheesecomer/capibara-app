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
                ActionSheetButton.CreateButton("Google", () => this.OAuthSignIn.Invoke(OAuthProvider.Google).Wait()),
                ActionSheetButton.CreateButton("Twitter", () => this.OAuthSignIn.Invoke(OAuthProvider.Twitter).Wait()),
                ActionSheetButton.CreateButton("LINE", () => this.OAuthSignIn.Invoke(OAuthProvider.Line).Wait())
            };

            // SignUpWithSnsCommand
            this.SignUpWithSnsCommand = this.SignUpCommand
                .CanExecuteChangedAsObservable()
                .Select(x => this.SignUpCommand.CanExecute() && this.Nickname.Value.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);

            this.SignUpWithSnsCommand.Subscribe(
                () => this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", actionSheetButtons));

            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(this.RefreshAsync);
        }

        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>(string.Empty);

        public AsyncReactiveCommand SignUpCommand { get; }

        public AsyncReactiveCommand SignUpWithSnsCommand { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public ISignUpUseCase SignUpUseCase { get; set; }

        public IOAuthSignIn OAuthSignIn { get; set; }

        public IRefreshSessionUseCase RefreshSessionUseCase { get; set; }

        public IHasSessionUseCase HasSessionUseCase { get; set; }

        protected Task RefreshAsync()
        {
            return this.HasSessionUseCase
                .Invoke()
                .Where(v => v)
                .FirstAsync()
                .SelectMany(_ => Observable.FromAsync(this.RefreshSessionUseCase.Invoke))
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
                .RetryWhen(x =>
                    x.ObserveOn(this.SchedulerProvider.UI)
                        .SelectMany(e =>
                            this.DisplayErrorAlertAsync(e)
                                .ToObservable()
                                .Select(v => new Pair<Exception, bool>(e, v)))
                        .SelectMany(v => v.Second
                            ? Observable.Return(Unit.Default)
                            : Observable.Throw<Unit>(v.First)))
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task SignUpAsync()
        {
            return Observable
                .FromAsync(() => SignUpUseCase.Invoke(this.Nickname.Value))
                .Select(_ => Unit.Default)
                .RetryWhen(x =>
                    x.ObserveOn(this.SchedulerProvider.UI)
                        .SelectMany(e =>
                            this.DisplayErrorAlertAsync(e)
                                .ToObservable()
                                .Select(v => new Pair<Exception, bool>(e, v)))
                        .SelectMany(v => v.Second
                            ? Observable.Return(Unit.Default)
                            : Observable.Throw<Unit>(v.First)))
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }
    }
}
