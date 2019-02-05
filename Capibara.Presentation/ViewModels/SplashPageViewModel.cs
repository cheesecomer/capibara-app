using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class SplashPageViewModel: ViewModelBase
    {
        private readonly double millisecondPerFrame = 10d;

        public SplashPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.RefreshCommandTask = this.RefreshAsync());

            this.LogoTopMargin.AddTo(this.Disposable);
            this.LogoScale.AddTo(this.Disposable);
            this.LogoOpacity.AddTo(this.Disposable);
        }

        public IHasSessionUseCase HasSessionUseCase { get; set; }

        public IRefreshSessionUseCase RefreshSessionUseCase { get; set; }

        public ReactiveProperty<double> LogoTopMargin { get; }
            = new ReactiveProperty<double>(180);

        public ReactiveProperty<double> LogoScale { get; }
            = new ReactiveProperty<double>(1);

        public ReactiveProperty<double> LogoOpacity { get; }
            = new ReactiveProperty<double>(1);

        public AsyncReactiveCommand RefreshCommand { get; }

        public Task RefreshCommandTask { get; private set; }

        protected Task RefreshAsync()
        {
            return this.HasSessionUseCase
                .Invoke()
                .FirstAsync()
                .SelectMany(v => v ? this.ToFloorMapPage() : this.ToSignUpPage())
                .ToTask()
                ;
        }

        private IObservable<Unit> ToSignUpPage()
        {
            return this.LogoTopMarginChangeAsync()
                .SelectMany(x => this.NavigationService.NavigateAsync("SignUpPage", animated: false))
                .Select(_ => Unit.Default);
        }

        private IObservable<Unit> ToFloorMapPage()
        {
            return Observable.FromAsync(this.RefreshSessionUseCase.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .ObserveOn(this.SchedulerProvider.UI)
                .SelectMany(user => this.LogoOpacityChangeAsync().Zip(this.LogoScaleChangeAsync(), (x, y) => user))
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

                    return this.NavigationService.NavigateAsync(pageName, parameters, animated: false).ToObservable();
                })
                .Select(_ => Unit.Default)
                .Catch((UnauthorizedException _) => this.ToSignUpPage())
                .RetryWhen(x =>
                    x.ObserveOn(this.SchedulerProvider.UI)
                        .SelectMany(e =>
                            this.DisplayErrorAlertAsync(e)
                                .ToObservable()
                                .Select(v => new Pair<Exception, bool>(e, v)))
                        .SelectMany(v => v.Second
                            ? Observable.Return(Unit.Default)
                            : Observable.Throw<Unit>(v.First)))
                .Catch((Exception _) => this.ApplicationExitUseCase.Invoke().ToObservable());
        }

        private IObservable<Unit> LogoTopMarginChangeAsync()
        {
            var initialValue = this.LogoTopMargin.Value;
            var step = (initialValue - 20d) / (500d / millisecondPerFrame);

            return Observable
                .Interval(TimeSpan.FromMilliseconds(millisecondPerFrame), this.SchedulerProvider.Delay)
                .TakeWhile(_ => this.LogoTopMargin.Value > 20)
                .Select(x => initialValue - (x + 1) * step)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(v => { this.LogoTopMargin.Value = v; }, () => { this.LogoTopMargin.Value = 20; })
                .Aggregate((x, y) => 0d)
                .Select(_ => Unit.Default);
        }

        private IObservable<Unit> LogoScaleChangeAsync()
        {
            var initialValue = this.LogoScale.Value;
            var step = 2d / (500d / millisecondPerFrame);

            return Observable
                .Interval(TimeSpan.FromMilliseconds(millisecondPerFrame), this.SchedulerProvider.Delay)
                .TakeWhile(_ => this.LogoScale.Value < 3)
                .Select(x => initialValue + (x + 1) * step)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(v => { this.LogoScale.Value = v; }, () => { this.LogoScale.Value = 3; })
                .Aggregate((x, y) => 0d)
                .Select(_ => Unit.Default);
        }

        private IObservable<Unit> LogoOpacityChangeAsync()
        {
            var initialValue = this.LogoOpacity.Value;
            var step = 1d / (500d / millisecondPerFrame);

            return Observable
                .Interval(TimeSpan.FromMilliseconds(millisecondPerFrame), this.SchedulerProvider.Delay)
                .TakeWhile(_ => this.LogoOpacity.Value > 0)
                .Select(x => initialValue - (x + 1) * step)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(v => { this.LogoOpacity.Value = v; }, () => { this.LogoOpacity.Value = 0; })
                .Aggregate((x, y) => 0d)
                .Select(_ => Unit.Default);
        }
    }
}
