using System;
using System.Threading.Tasks;
using System.Reactive.Disposables;

using Capibara.Services;
using Capibara.Models;

using Unity;
using Unity.Attributes;

using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.ViewModels
{
    public static class ParameterNames
    {
        public const string Model = "ParameterNames.Model";

        public const string Title = "ParameterNames.Title";

        public const string Url = "ParameterNames.Url";
    }

    public class ViewModelBase : BindableBase, INavigationAware
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

        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        public virtual void OnNavigatedTo(NavigationParameters parameters) { }

        public virtual void OnNavigatingTo(NavigationParameters parameters) { }

        protected virtual void OnContainerChanged() { }

        protected virtual async void OnFail(object sender, FailEventArgs args)
        {
            await this.DisplayErrorAlertAsync(args.Error);
        }

        protected virtual async Task DisplayErrorAlertAsync(Exception exception)
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
                await this.PageDialogService.DisplayAlertAsync("申し訳ございません！", "通信エラーです。再度実行してください。", "閉じる");
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
            base.OnContainerChanged();

            if (this.Container != null)
                this.Model.BuildUp(this.Container);
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            this.Model.Restore(parameters?.TryGetValue<TModel>(ParameterNames.Model) ?? this.Model); 
        }
    }
}
