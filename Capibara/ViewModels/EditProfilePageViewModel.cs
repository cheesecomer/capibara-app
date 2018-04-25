using System;
using System.IO;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class EditProfilePageViewModel : UserViewModel
    {
        public AsyncReactiveCommand CommitCommand { get; }

        public AsyncReactiveCommand ChangePhotoCommand { get; }

        public AsyncReactiveCommand CooperationSnsCommand { get; }

        public EditProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            // CommitCommand
            this.CommitCommand = this.Nickname.Select(x => x.ToSlim().IsPresent()).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.CommitCommand.Subscribe(async () => {
                if (!this.Model.IconBase64.IsNullOrEmpty())
                {
                    var canReward = await this.PageDialogService.DisplayAlertAsync(
                        string.Empty,
                        "動画広告を視聴して\r\n" +
                        "プロフィール画像を更新しよう！",
                        "視聴する",
                        "閉じる");
                    if (!canReward) return;

                    var completed = await this.RewardedVideoService.DisplayRewardedVideo();
                    if (!completed) return;
                }

                await this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit());
            });

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
                    var bytes = await this.PickupPhotoService.DisplayAlbumAsync(Services.CropMode.Square);
                    if (bytes != null)
                    {
                        this.Icon.Value = this.ImageSourceFactory.FromStream(() => new MemoryStream(bytes));
                        this.Model.IconBase64 = Convert.ToBase64String(bytes);
                    }
                });
                var takeButton = ActionSheetButton.CreateButton("カメラで撮影", () => { });
                await this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, pickupButton);
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
            var query = $"?user_id={this.Model.Id}&access_token={this.IsolatedStorage.AccessToken}";
            var url = Path.Combine(this.Environment.OAuthBaseUrl, provider.ToString().ToLower());
            this.SnsLoginService.Open(url + query);
        }
    }
}
