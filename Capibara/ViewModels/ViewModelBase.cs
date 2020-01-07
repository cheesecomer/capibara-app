using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Capibara.Forms;
using Capibara.Models;
using Capibara.Services;
using Plugin.GoogleAnalytics.Abstractions;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Unity;

namespace Capibara.ViewModels
{
    public static class ParameterNames
    {
        public const string Model = "ParameterNames.Model";

        public const string Title = "ParameterNames.Title";

        public const string Url = "ParameterNames.Url";
    }

    public class ViewModelBase : BindableBase, INavigationAware, IApplicationLifecycleAware, IDisposable
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

        [Unity.Dependency]
        public IProgressDialogService ProgressDialogService { get; set; }

        [Unity.Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        [Unity.Dependency]
        public IEnvironment Environment { get; set; }

        [Unity.Dependency(UnityInstanceNames.CurrentUser)]
        public User CurrentUser { get; set; }

        [Unity.Dependency]
        public IDeviceService DeviceService { get; set; }

        [Unity.Dependency]
        public Net.IRequestFactory RequestFactory { get; set; }

        [Unity.Dependency]
        public ITaskService TaskService { get; set; }

        [Unity.Dependency]
        public IApplicationService ApplicationService { get; set; }

        [Unity.Dependency]
        public ITracker Tracker { get; set; }

        [Unity.Dependency]
        public IBalloonService BalloonService { get; set; }

        [Unity.Dependency]
        public ISnsLoginService SnsLoginService { get; set; }

        [Unity.Dependency]
        public IRewardedVideoService RewardedVideoService { get; set; }

        [Unity.Dependency]
        public IImageSourceFactory ImageSourceFactory { get; set; }

        [Unity.Dependency]
        public IPickupPhotoService PickupPhotoService { get; set; }

        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [Unity.Dependency]
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

        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            this.OnResume();
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters) { }

        protected virtual void OnContainerChanged() { }

        protected EventHandler<FailEventArgs> OnFail(Func<Task> retryFunction, Action cancelAction = null)
        {
            return async (s, args) =>
            {
                var taskSource = new TaskCompletionSource<bool>();

                this.DeviceService.BeginInvokeOnMainThread(async () =>
                {
                    var needRetry = await this.DisplayErrorAlertAsync(args.Error);
                    if (needRetry)
                    {
                        retryFunction?.Invoke();
                    }
                    else
                    {
                        cancelAction?.Invoke();
                    }


                    taskSource.TrySetResult(true);
                });

                await taskSource.Task;
            };
        }

        protected virtual async Task<bool> DisplayErrorAlertAsync(Exception exception)
        {
            if (exception is Net.HttpUnauthorizedException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "再度ログインしてください", "閉じる");
                await this.NavigationService.NavigateAsync("/SignUpPage");
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
                return needRetry;
            }

            return false;
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
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

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            this.Model.Restore(parameters?.TryGetValue<TModel>(ParameterNames.Model) ?? this.Model);

            base.OnNavigatingTo(parameters);
        }
    }
}
