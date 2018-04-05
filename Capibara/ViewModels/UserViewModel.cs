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

using UnityDependency = Unity.Attributes.DependencyAttribute;

namespace Capibara.ViewModels
{
    public class UserViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public ReactiveProperty<ImageSource> Icon { get; }

        public ReactiveProperty<ImageSource> IconThumbnail { get; }

        public ReactiveProperty<bool> IsBlock { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand EditCommand { get; }

        public AsyncReactiveCommand CommitCommand { get; }

        public AsyncReactiveCommand BlockCommand { get; }

        public AsyncReactiveCommand ChangePhotoCommand { get; }

        public AsyncReactiveCommand ReportCommand { get; }

        public AsyncReactiveCommand CooperationSnsCommand { get; }

        protected override bool NeedTrackingView => !this.Model.IsOwn;

        protected override string OptionalScreenName => $"/{this.Model.Id}";

        [UnityDependency]
        public IPickupPhotoService PickupPhotoService { get; set; }

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

            this.IconThumbnail = this.Model
                .ObserveProperty(x => x.IconThumbnailUrl)
                .Select(x => x.IsNullOrEmpty() ? null : ImageSource.FromUri(new Uri(x)))
                .ToReactiveProperty();
            
            this.IsBlock = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsBlock)
                .AddTo(this.Disposable);

            this.Nickname.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Nickname)));
            this.Biography.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Biography)));
            this.Icon.Subscribe(_ => this.RaisePropertyChanged(nameof(this.Icon)));
            this.IsBlock.Subscribe(_ => this.RaisePropertyChanged(nameof(this.IsBlock)));

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Refresh()));

            this.Model.RefreshFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Refresh()));

            // EditCommand
            this.EditCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.EditCommand.Subscribe(async () => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.NavigateAsync("EditProfilePage", parameters);
            });

            // CommitCommand
            this.CommitCommand = this.Nickname.Select(x => x.ToSlim().IsPresent()).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.CommitCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit()));

            this.Model.CommitSuccess += async (sender, e) => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.GoBackAsync(parameters);
            };

            this.Model.CommitFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit()));

            // ChangePhotoCommand
            this.ChangePhotoCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ChangePhotoCommand.Subscribe(async () => {
                var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });
                var deleteButton = ActionSheetButton.CreateDestroyButton("削除", () => this.Icon.Value = null);
                var pickupButton = ActionSheetButton.CreateButton("アルバムから選択", async () => {
                    var bytes = await this.PickupPhotoService.DisplayAlbumAsync();
                    if (bytes != null)
                    {
                        this.Icon.Value = ImageSource.FromStream(() => new MemoryStream(bytes));
                        this.Model.IconBase64 = Convert.ToBase64String(bytes);
                    }
                });
                var takeButton = ActionSheetButton.CreateButton("カメラで撮影", () => { });
                await this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, pickupButton);
            });

            // BlockCommand
            this.BlockCommand = this.IsBlock.Select(x => !x).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.BlockCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Block()));

            this.Model.BlockFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Block()));

            // ReportCommand
            this.ReportCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ReportCommand.Subscribe(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, this.Model);
                return this.NavigationService.NavigateAsync("ReportPage", parameters);
            });

            // SignUpWithSnsCommand
            this.CooperationSnsCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CooperationSnsCommand.Subscribe(async () => {
                var buttons = new[] {
                    ActionSheetButton.CreateCancelButton("キャンセル", () => { }),
                    ActionSheetButton.CreateButton("Google", () => this.OpenOAuthUri(OAuthProvider.Google)),
                    ActionSheetButton.CreateButton("Twitter", () => this.OpenOAuthUri(OAuthProvider.Twitter)),
                    ActionSheetButton.CreateButton("LINE", () => this.OpenOAuthUri(OAuthProvider.Line))
                };

                await this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", buttons);
            });
        }

        private void OpenOAuthUri(OAuthProvider provider)
        {
            var uri = new Uri(
                Path.Combine(this.Environment.OAuthBaseUrl, provider.ToString().ToLower()) + 
                $"?user_id={this.Model.Id}&access_token={this.IsolatedStorage.AccessToken}");
            this.DeviceService.OpenUri(uri);
        }
    }
}
