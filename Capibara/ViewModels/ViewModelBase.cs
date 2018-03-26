using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Disposables;

using Capibara.Services;
using Capibara.Models;

using Unity;
using Unity.Attributes;

using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

using Plugin.GoogleAnalytics.Abstractions;

using Prism.AppModel;

namespace Capibara.ViewModels
{
    public static class ParameterNames
    {
        public const string Model = "ParameterNames.Model";

        public const string Title = "ParameterNames.Title";

        public const string Url = "ParameterNames.Url";
    }

    public class ViewModelBase : BindableBase, INavigationAware, IApplicationLifecycleAware
    {
        private IUnityContainer container;

        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this.NavigationService = navigationService;
            this.PageDialogService = pageDialogService;
        }

        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();

        protected INavigationService NavigationService { get; }

        protected IPageDialogService PageDialogService { get; }

        protected virtual bool NeedTrackingView { get; } = true;

        protected virtual string OptionalScreenName { get; } = string.Empty;

        [Dependency]
        public IProgressDialogService ProgressDialogService { get; set; }

        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        [Dependency]
        public IEnvironment Environment { get; set; }

        [Dependency(UnityInstanceNames.CurrentUser)]
        public User CurrentUser { get; set; }

        [Dependency]
        public IDeviceService DeviceService { get; set; }

        [Dependency]
        public Net.IRequestFactory RequestFactory { get; set; }

        [Dependency]
        public ITaskService TaskService { get; set; }

        [Dependency]
        public IApplicationService ApplicationService { get; set; }

        [Dependency]
        public ITracker Tracker { get; set; }

        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [Dependency]
        public IUnityContainer Container
        {
            get => this.container;
            set
            {
                this.container = value;

                this.OnContainerChanged();
            }
        }

        public virtual void OnResume()
        {
            if (this.NeedTrackingView)
            {
                var screenName = this.GetType().Name.Replace("PageViewModel", string.Empty).Replace("ViewModel", string.Empty);
                this.Tracker.SendView(screenName + this.OptionalScreenName);
            }
        }

        public virtual void OnSleep()
        {
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            if (this.DeviceService?.DeviceRuntimePlatform == Xamarin.Forms.Device.iOS && !(this is MainPageViewModel))
            {
                this.OnResume();
            }
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters) { }

        protected virtual void OnContainerChanged() { }

        protected virtual EventHandler<FailEventArgs> OnFail(Func<Task> func)
        {
            return async (s, args) =>
            {
                await this.DisplayErrorAlertAsync(args.Error, func);
            };
        }

        protected virtual async Task DisplayErrorAlertAsync(Exception exception, Func<Task> func)
        {
            if (exception is Net.HttpUnauthorizedException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "再度ログインしてください", "閉じる");
                await this.NavigationService.NavigateAsync("/SignInPage");
            }
            else if (exception is Net.HttpForbiddenException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "不正なアクセスです。再度操作をやり直してください。", "閉じる");
                await this.NavigationService.GoBackAsync();
            }
            else if (exception is Net.HttpNotFoundException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "データが見つかりません。再度操作をやり直してください。", "閉じる");
                await this.NavigationService.GoBackAsync();
            }
            else if (exception is Net.HttpUpgradeRequiredException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "最新のアプリが公開されています！アップデートを行ってください。", "閉じる");
                this.DeviceService.OpenUri(new Uri(this.ApplicationService.StoreUrl));
            }
            else if (exception is Net.HttpServiceUnavailableException)
            {
                await this.PageDialogService.DisplayAlertAsync("申し訳ございません！", "現在メンテナンス中です。時間を置いて再度お試しください。", "閉じる");
                this.ApplicationService.Exit();
            }
            else
            {
                var needRetry =
                    await this.PageDialogService.DisplayAlertAsync(
                        "申し訳ございません！",
                        "通信エラーです。リトライしますか？。",
                        "リトライ",
                        "閉じる");

                if (needRetry) await func.Invoke();
            }
        }
    }

    public class ViewModelBase<TModel> : ViewModelBase where TModel : Models.ModelBase<TModel>, new()
    {
        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, TModel model)
            : base(navigationService, pageDialogService)
        {
            this.Model = model ?? new TModel();
        }

        public TModel Model { get; }

        protected override void OnContainerChanged()
        {
            if (this.Container != null)
                this.Model.BuildUp(this.Container);

            base.OnContainerChanged();
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            this.Model.Restore(parameters?.TryGetValue<TModel>(ParameterNames.Model) ?? this.Model);

            base.OnNavigatingTo(parameters);
        }
    }
}
