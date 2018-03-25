using System;
using System.Threading.Tasks;
using System.Linq;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class BlockUsersPageViewModel : ViewModelBase
    {
        public ReactiveCollection<Block> Blocks { get; } = new ReactiveCollection<Block>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Block> ItemTappedCommand { get; }

        public BlockUsersPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<Block>();
        }

        private async Task Refresh()
        {
            var request = this.RequestFactory.BlocksIndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                this.Blocks.Clear();

                response.Blocks?.ForEach(x => this.Blocks.Add(x.BuildUp(this.Container)));

            }
            catch (Exception e)
            {
                await this.DisplayErrorAlertAsync(e, () => this.Refresh());
            }
        }
    }
}
