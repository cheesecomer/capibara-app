using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class InboxPageViewModel: ViewModelBase
    {
        public InboxPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand()
                .WithSubscribe(this.RefreshAsync)
                .AddTo(this.Disposable);

            this.ItemTappedCommand = new AsyncReactiveCommand<Message>()
                .WithSubscribe(this.NavigateAsync)
                .AddTo(this.Disposable);
        }

        public IFetchDirectMessageThreadUseCase FetchDirectMessageThreadUseCase { get; set; }

        public ReactiveCollection<Message> Threads { get; } = new ReactiveCollection<Message>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Message> ItemTappedCommand { get; }

        private Task RefreshAsync()
        {
            return Observable
                .Defer(FetchDirectMessageThreadUseCase.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .Do(directMessageThreads =>
                {
                    this.Threads.Clear();
                    directMessageThreads?.ForEach(this.Threads.Add);
                })
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task NavigateAsync(Message message)
        {
            var parameters =
                new NavigationParameterBuilder { Model = message }
                    .Build();

            return this.NavigationService.NavigateAsync("DirectMessagePage", parameters);
        }
    }
}
