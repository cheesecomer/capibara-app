using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class UserViewModel : ViewModelBase<User>
    {
        public UserViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Nickname = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Nickname)
                .AddTo(this.Disposable);

            this.Biography = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Biography)
                .AddTo(this.Disposable);

            this.IconUrl = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IconUrl)
                .AddTo(this.Disposable);

            this.IconThumbnailUrl = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IconThumbnailUrl)
                .AddTo(this.Disposable);

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.FetchUserUseCase.Invoke(this.Model));
        }

        public IFetchUserUseCase FetchUserUseCase { get; set; }

        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public ReactiveProperty<string> IconUrl { get; }

        public ReactiveProperty<string> IconThumbnailUrl { get; }

        public AsyncReactiveCommand RefreshCommand { get; }
    }
}
