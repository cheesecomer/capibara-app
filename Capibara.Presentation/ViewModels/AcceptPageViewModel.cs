using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class AcceptPageViewModel : ViewModelBase<User>
    {
        public AcceptPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.subject.Subscribe(Observer.Create<Pair<WebPage, string>>(x => this.isLoaded.OnNext(false))).AddTo(this.Disposable);

            this.nextCommand = new AsyncReactiveCommand(this.isLoaded)
                .AddTo(this.Disposable);
            this.nextCommand.Subscribe(this.NextAsync);

            this.agreeCommand = new AsyncReactiveCommand(this.isLoaded)
                .AddTo(this.Disposable);
            this.agreeCommand.Subscribe(this.AgreeAsync);

            this.Title = subject
                .Select(this.TitleSelector)
                .ToReadOnlyReactiveProperty(string.Empty)
                .AddTo(this.Disposable);

            this.CurrentCommand = subject
                .Select(this.CurrentCommandSelector)
                .ToReadOnlyReactiveProperty()
                .AddTo(this.Disposable);

            this.CurrentCommandTitle = subject
                .Select(this.CurrentCommandTitleSelector)
                .ToReadOnlyReactiveProperty(string.Empty)
                .AddTo(this.Disposable);

            this.Url = subject
                .Select(x => x.Second)
                .ToReadOnlyReactiveProperty(string.Empty)
                .AddTo(this.Disposable);

            this.OpenCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.OpenCommand.Subscribe(this.OpenAsync);

            this.LoadedCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.LoadedCommand.Subscribe(this.LoadedAsync);
        }

        private readonly BehaviorSubject<bool> isLoaded = new BehaviorSubject<bool>(false);

        private readonly AsyncReactiveCommand nextCommand;

        private readonly AsyncReactiveCommand agreeCommand;

        private readonly ReplaySubject<Pair<WebPage, string>> subject = new ReplaySubject<Pair<WebPage, string>>();

        [Unity.Dependency]
        public IAcceptUseCase AcceptUseCase { get; set; }

        [Unity.Dependency]
        public IGetWebPageUrlUseCase GetWebPageUrlUseCase { get; set; }

        public AsyncReactiveCommand OpenCommand { get; }

        public AsyncReactiveCommand LoadedCommand { get; }

        public ReadOnlyReactiveProperty<string> Url { get; }

        public ReadOnlyReactiveProperty<string> Title { get; }

        public ReadOnlyReactiveProperty<AsyncReactiveCommand> CurrentCommand { get; }

        public ReadOnlyReactiveProperty<string> CurrentCommandTitle { get; }

        private Task OpenAsync()
        {
            return Observable.Defer(() => this.GetWebPageUrlUseCase.Invoke(WebPage.Terms))
                .SubscribeOn(this.SchedulerProvider.IO)
                .SelectMany(
                    x => Observable.Start(() =>
                    {
                        this.subject.OnNext(new Pair<WebPage, string>(WebPage.Terms, x));
                    },
                    this.SchedulerProvider.UI))
                .ToTask();
        }

        private Task LoadedAsync()
        {
            return Observable.Start(
                () => this.isLoaded.OnNext(true),
                this.SchedulerProvider.UI)
                .ToTask();
        }

        private Task NextAsync()
        {
            return Observable.Defer(() => this.GetWebPageUrlUseCase.Invoke(WebPage.PrivacyPolicy))
                .SubscribeOn(this.SchedulerProvider.IO)
                .SelectMany(
                    x => Observable.Start(() =>
                    {
                        this.subject.OnNext(new Pair<WebPage, string>(WebPage.PrivacyPolicy, x));
                    },
                    this.SchedulerProvider.UI))
                .ToTask();
        }

        private Task AgreeAsync()
        {
            return Observable.Defer(this.AcceptUseCase.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .SelectMany(_ =>
                {
                    return Observable.FromAsync(
                        () => this.NavigationService.NavigateAsync("/NavigationPage/MainPage"),
                        this.SchedulerProvider.UI);
                })
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private string TitleSelector(Pair<WebPage, string> value)
        {
            switch (value.First)
            {
                case WebPage.Terms:
                    return "利用規約の同意";
                case WebPage.PrivacyPolicy:
                    return "プライバシーポリシーの同意";
                default:
                    return string.Empty;
            }
        }

        private AsyncReactiveCommand CurrentCommandSelector(Pair<WebPage, string> value)
        {
            switch (value.First)
            {
                case WebPage.Terms:
                    return this.nextCommand;
                case WebPage.PrivacyPolicy:
                    return this.agreeCommand;
                default:
                    return null;
            }
        }

        private string CurrentCommandTitleSelector(Pair<WebPage, string> value)
        {
            switch (value.First)
            {
                case WebPage.Terms:
                    return "次へ";
                case WebPage.PrivacyPolicy:
                    return "同意する";
                default:
                    return string.Empty;
            }
        }
    }
}
