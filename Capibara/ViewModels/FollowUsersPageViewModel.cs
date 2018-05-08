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
    public class FollowUsersPageViewModel : ViewModelBase
    {
        public ReactiveCollection<UserViewModel> FollowUsers { get; } = new ReactiveCollection<UserViewModel>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<UserViewModel> ItemTappedCommand { get; }

        public FollowUsersPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<UserViewModel>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                var parameters = new NavigationParameters { { ParameterNames.Model, x.Model } };
                await this.NavigationService.NavigateAsync("UserProfilePage", parameters);
            });
        }

        private async Task Refresh()
        {
            var request = this.RequestFactory.FollowsIndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                response.Follows?.Select(x => x.Target).ForEach(x =>
                {
                    if (this.FollowUsers.Any(y => y.Model.Id == x.Id))
                    {
                        this.FollowUsers.First(y => y.Model.Id == x.Id).Model.Restore(x);
                    }
                    else
                    {
                        this.FollowUsers.Add(new UserViewModel(this.NavigationService, this.PageDialogService, x));
                    }
                });

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
