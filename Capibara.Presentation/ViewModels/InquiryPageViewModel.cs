using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class InquiryPageViewModel: ViewModelBase<Inquiry>
    {
        public ICreateInquiryUseCase CreateInquiryUseCase { get; set; }

        public ReactiveProperty<string> Email { get; }

        public ReactiveProperty<string> Message { get; }

        public AsyncReactiveCommand SubmitCommand { get; }

        public InquiryPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService, new Inquiry())
        {
            this.Email = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Email)
                .AddTo(this.Disposable);

            this.Message = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Message)
                .AddTo(this.Disposable);

            this.SubmitCommand = new[] { this.Email, this.Message }
                .CombineLatest(x => x.All(v => v.IsPresent()))
                .ToAsyncReactiveCommand()
                .WithSubscribe(this.SubmitAsync)
                .AddTo(this.Disposable);
        }

        private Task SubmitAsync()
        {
            return Observable
                .Defer(() => this.CreateInquiryUseCase.Invoke(this.Model))
                .SubscribeOn(this.SchedulerProvider.IO)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .SelectMany(_ => this.NavigationService.GoBackAsync())
                .SubscribeOn(this.SchedulerProvider.UI)
                .Select(_ => Unit.Default)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

    }
}
