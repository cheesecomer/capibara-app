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
        public ReadOnlyReactiveCollection<UserViewModel> Participants { get; }

        public AsyncReactiveCommand<UserViewModel> ItemTappedCommand { get; }

        protected override string OptionalScreenName => $"/{this.Model.Id}";

        public ParticipantsPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Participants = this.Model
                .Participants
                .ToReadOnlyReactiveCollection((x) => new UserViewModel(model: x).BuildUp(this.Container));

            this.ItemTappedCommand = new AsyncReactiveCommand<UserViewModel>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                if (x.Model.Id == this.IsolatedStorage.UserId)
                {
                    var parameters = new NavigationParameters { { ParameterNames.Model, this.CurrentUser } };
                    await this.NavigationService.NavigateAsync("MyProfilePage", parameters);
                }
                else
                {
                    var parameters = new NavigationParameters { { ParameterNames.Model, x.Model } };
                    await this.NavigationService.NavigateAsync("UserProfilePage", parameters);
                }
            });
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.Participants.ForEach(x => x.BuildUp(this.Container));
        }
    }
}
