using System;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class UserProfilePageViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand EditCommand { get; }

        public AsyncReactiveCommand CommitCommand { get; }

        public UserProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Nickname = this.Model
                .ObserveProperty(x => x.Nickname)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Biography = this.Model
                .ObserveProperty(x => x.Biography)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayAlertAsync(this.Model.Refresh()));

            // EditCommand
            this.EditCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.EditCommand.Subscribe(async () => {
                var parameters = new NavigationParameters() { { ParameterNames.Model, this.Model } };
                await this.NavigationService.NavigateAsync("EditProfilePage", parameters);
            });

            // CommitCommand
            this.CommitCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CommitCommand.Subscribe(() => this.ProgressDialogService.DisplayAlertAsync(this.Model.Commit()));
        }
    }
}
