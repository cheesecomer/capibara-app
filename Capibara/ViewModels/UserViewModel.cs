using System;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class UserViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public ReactiveProperty<ImageSource> Icon { get; }

        public ReactiveProperty<ImageSource> IconThumbnail { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        protected override bool NeedTrackingView => !this.Model.IsOwn;

        protected override string OptionalScreenName => $"/{this.Model.Id}";

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

            this.Icon = new ReactiveProperty<ImageSource>();
            this.Model.ObserveProperty(x => x.IconUrl).Subscribe(x => this.Icon.Value = x);

            this.IconThumbnail = new ReactiveProperty<ImageSource>();
            this.Model.ObserveProperty(x => x.IconThumbnailUrl).Subscribe(x => this.IconThumbnail.Value = x);

            this.Nickname.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Nickname)));
            this.Biography.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Biography)));
            this.Icon.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Icon)));

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Refresh()));

            this.Model.RefreshFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Refresh()));
        }
    }
}
