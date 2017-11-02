using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class ParticipantsPageViewModel : ViewModelBase<Room>
    {
        public ReadOnlyReactiveCollection<User> Participants { get; }

        public AsyncReactiveCommand<User> ItemTappedCommand { get; }

        public ParticipantsPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Participants =
                    this.Model.Participants.ToReadOnlyReactiveCollection();
            
            this.ItemTappedCommand = new AsyncReactiveCommand<User>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                if (x.Id == this.IsolatedStorage.UserId)
                {
                    var parameters = new NavigationParameters { { ParameterNames.Model, this.CurrentUser } };
                    await this.NavigationService.NavigateAsync("MyProfilePage", parameters);
                }
                else
                {
                    var parameters = new NavigationParameters { { ParameterNames.Model, x } };
                    await this.NavigationService.NavigateAsync("UserProfilePage", parameters);
                }
            });
        }
    }
}
