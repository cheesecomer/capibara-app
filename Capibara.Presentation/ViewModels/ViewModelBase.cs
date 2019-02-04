using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Capibara.Domain.UseCases;
using Capibara.Reactive;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    public abstract class ViewModelBase: IDisposable
    {
        private bool disposedValue;

        protected ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this.NavigationService = navigationService;
            this.PageDialogService = pageDialogService;
        }

        #region Public Properties

        public ISchedulerProvider SchedulerProvider { get; set; }

        public IApplicationExitUseCase ApplicationExitUseCase { get; set; }

        #endregion

        #region Protected Properties

        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();

        protected INavigationService NavigationService { get; }

        protected IPageDialogService PageDialogService { get; }

        #endregion

        public Task<bool> DisplayErrorAlertAsync(Exception exception)
        {
            return this.PageDialogService.DisplayAlertAsync(
                        "申し訳ございません！",
                        "通信エラーです。リトライしますか？。",
                        "リトライ",
                        "閉じる");
        }

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
    }
}
