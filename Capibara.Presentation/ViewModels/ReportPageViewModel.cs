using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
    public class ReportPageViewModel : ViewModelBase<Report>
    {
        public ICreateReportUseCase CreateReportUseCase { get; set; }

        public ReactiveCollection<ReportReason> ReportReasons { get; } = new ReactiveCollection<ReportReason>()
        {
            ReportReason.Spam,
            ReportReason.AbusiveOrHatefulSpeech,
            ReportReason.AbusiveOrHatefulImage,
            ReportReason.ObsceneSpeech,
            ReportReason.ObsceneImage,
            ReportReason.Other
        };

        public ReactiveProperty<ReportReason?> Reason { get; }

        public ReactiveProperty<string> Message { get; }

        public AsyncReactiveCommand SubmitCommand { get; }

        public ReportPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Report model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Message = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Message)
                .AddTo(this.Disposable);

            this.Reason = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Reason)
                .AddTo(this.Disposable);

            this.SubmitCommand = new[]
            {
                this.Reason.Select(x => x.HasValue),
                this.Message.Select(x => x.ToSlim().IsPresent())
            }
            .CombineLatest(x => x.All(v => v))
            .ToAsyncReactiveCommand()
            .WithSubscribe(this.CreateReportAsync)
            .AddTo(this.Disposable);
        }

        private Task CreateReportAsync()
        {
            return Observable.Defer(() => this.CreateReportUseCase.Invoke(this.Model))
                .SubscribeOn(this.SchedulerProvider.IO)
                .SelectMany(x => this.NavigationService.GoBackAsync())
                .SubscribeOn(this.SchedulerProvider.UI)
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }
    }
}
