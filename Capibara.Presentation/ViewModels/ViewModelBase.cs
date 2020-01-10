using System;
using System.Reactive.Disposables;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Capibara.Reactive;
using Capibara.Services;
using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;
using Unity;

namespace Capibara.Presentation.ViewModels
{
    public abstract class ViewModelBase: IDisposable, IInitialize, IApplicationLifecycleAware
    {
        private bool disposedValue;

        private IUnityContainer container;

        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this.NavigationService = navigationService;
            this.PageDialogService = pageDialogService;
        }

        #region Public Properties

        [Dependency]
        public ISchedulerProvider SchedulerProvider { get; set; }

        [Dependency]
        public IApplicationExitUseCase ApplicationExitUseCase { get; set; }

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

        [Dependency]
        public IProgressDialogService ProgressDialogService { get; set; }

        [Dependency]
        public ISnsLoginService SnsLoginService { get; set; }

        [Dependency]
        public IRewardedVideoService RewardedVideoService { get; set; }

        #endregion

        #region Protected Properties

        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();

        protected INavigationService NavigationService { get; }

        protected IPageDialogService PageDialogService { get; }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            if (!disposedValue)
            {
                this.Disposable.Dispose();

                disposedValue = true;
            }
        }

        #endregion

        #region IInitialize

        public virtual void Initialize(INavigationParameters parameters) { }

        #endregion

        #region IApplicationLifecycleAware

        public virtual void OnResume() { }

        public virtual void OnSleep() { }

        #endregion

        protected virtual void OnContainerChanged() { }
    }

    public class ViewModelBase<TModel> : ViewModelBase where TModel : Domain.Models.ModelBase<TModel>, new()
    {
        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, TModel model)
            : base(navigationService, pageDialogService)
        {
            this.Model = model ?? new TModel();
        }

        public TModel Model { get; }

        public override void Initialize(INavigationParameters parameters)
        {
            this.Model.Restore(parameters?.TryGetValue<TModel>(ParameterNames.Model) ?? this.Model);

            base.Initialize(parameters);
        }
    }
}
