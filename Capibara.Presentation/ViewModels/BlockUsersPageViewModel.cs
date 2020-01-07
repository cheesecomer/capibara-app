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
    public class BlockUsersPageViewModel : ViewModelBase
    {
        public BlockUsersPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(this.RefreshAsync);
        }

        [Unity.Dependency]
        public IFetchBlockUsersUseCase FetchBlockUsersUseCase { get; set; }

        public ReactiveCollection<Block> Blocks { get; } = new ReactiveCollection<Block>();

        public AsyncReactiveCommand RefreshCommand { get; }

        private Task RefreshAsync()
        {
            return Observable.FromAsync(
                this.FetchBlockUsersUseCase.Invoke,
                this.SchedulerProvider.IO)
                .SelectMany(
                    blocks => Observable.Start(() =>
                    {
                        this.Blocks.Clear();

                        blocks?.ForEach(x => this.Blocks.Add(x));
                    },
                    this.SchedulerProvider.UI))
                .ToTask();
        }
    }
}
