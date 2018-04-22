using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class InboxPageViewModel: ViewModelBase
    {
        public ReactiveCollection<DirectMessageThreadViewModel> Threads { get; } =
            new ReactiveCollection<DirectMessageThreadViewModel>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<DirectMessageThreadViewModel> ItemTappedCommand { get; }

        public InboxPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<DirectMessageThreadViewModel>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                var parameters = new NavigationParameters { { ParameterNames.Model, x.Model } };
                await this.NavigationService.NavigateAsync("DirectMessagePage", parameters);
            });
        }

        private async Task Refresh()
        {
            var request = this.RequestFactory.DirectMessagesIndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                this.Threads.Clear();
                response.Threads?.ForEach(
                    x => this.Threads.Add(
                        new DirectMessageThreadViewModel(
                            this.NavigationService, 
                            this.PageDialogService, 
                            x)));

            }
            catch (Exception e)
            {
                if (await this.DisplayErrorAlertAsync(e))
                {
                    await this.Refresh();
                }
            }
        }
    }
}
