using System.Reactive.Disposables;

using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.ViewModels
{
    public static class ParameterNames
    {
        public const string Model = "ParameterNames.Model";
    }

    public class ViewModelBase<TModel> : BindableBase, INavigationAware where TModel : Models.ModelBase<TModel>, new()
    {
        private IUnityContainer container;

        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, TModel model)
        {
            this.NavigationService = navigationService;
            this.PageDialogService = pageDialogService;
            this.Model = model ?? new TModel();
        }

        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();

        protected INavigationService NavigationService { get; }

        protected IPageDialogService PageDialogService { get; }

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

                if (value.IsPresent())
                {
                    this.Model.BuildUp(this.Container);
                }
            }
        }

        public TModel Model { get; }

        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters?.ContainsKey(ParameterNames.Model) ?? false)
            {
                this.Model.Restore(parameters[ParameterNames.Model] as TModel);
            }
        }
    }
}
