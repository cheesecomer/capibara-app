using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Services;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class UserProfilePageViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public ReactiveProperty<ImageSource> Image { get; } = new ReactiveProperty<ImageSource>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand EditCommand { get; }

        public AsyncReactiveCommand CommitCommand { get; }

        public AsyncReactiveCommand ChangePhotoCommand { get; }

        [Microsoft.Practices.Unity.Dependency]
        public IPickupPhotoService PickupPhotoService { get; set; }

        public UserProfilePageViewModel(
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

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayAlertAsync(this.Model.Refresh()));

            // EditCommand
            this.EditCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.EditCommand.Subscribe(async () => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.NavigateAsync("EditProfilePage", parameters);
            });

            // CommitCommand
            this.CommitCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CommitCommand.Subscribe(() => this.ProgressDialogService.DisplayAlertAsync(this.Model.Commit()));

            // ChangePhotoCommand
            this.ChangePhotoCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ChangePhotoCommand.Subscribe(async () => {
                var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });
                var deleteButton = ActionSheetButton.CreateDestroyButton("削除", () => { });
                var pickupButton = ActionSheetButton.CreateButton("アルバムから選択", async () => {
                    var stream = await this.PickupPhotoService.DisplayAlbumAsync();
                    this.Image.Value = ImageSource.FromStream(() => stream);
                });
                var takeButton = ActionSheetButton.CreateButton("カメラで撮影", () => { });
                await this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, deleteButton, pickupButton, takeButton);
            });

            this.Model.CommitSuccess += async (sender, e) => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.GoBackAsync(parameters);
            };
        }
    }
}
