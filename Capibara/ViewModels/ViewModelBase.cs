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

            if (parameters?.ContainsKey(ParameterNames.Model) ?? false)
            {
                this.Model.Restore(parameters[ParameterNames.Model] as TModel);
            }
        }
    }
}
