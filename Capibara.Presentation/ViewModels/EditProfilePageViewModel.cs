using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class EditProfilePageViewModel : UserViewModel
    {
        public IUpdateProfileUseCase UpdateProfileUseCase { get; set; }

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
            this.CommitCommand.Subscribe(this.CommitAsync);

            //this.Model.CommitSuccess += async (sender, e) =>
            //{
            //    var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
            //    await this.NavigationService.GoBackAsync(parameters);
            //};

            //this.Model.CommitFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Commit()));

            //// ChangePhotoCommand
            //this.ChangePhotoCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            //this.ChangePhotoCommand.Subscribe(async () =>
            //{
            //    var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });
            //    var deleteButton = ActionSheetButton.CreateDestroyButton("削除", () => this.IconUrl.Value = null);
            //    var pickupButton = ActionSheetButton.CreateButton("アルバムから選択", async () =>
            //    {
            //        var bytes = await this.PickupPhotoService.DisplayAlbumAsync(Services.CropMode.Square);
            //        if (bytes != null)
            //        {
            //            this.Icon.Value = this.ImageSourceFactory.FromStream(() => new MemoryStream(bytes));
            //            this.Model.IconBase64 = Convert.ToBase64String(bytes);
            //        }
            //    });
            //    var takeButton = ActionSheetButton.CreateButton("カメラで撮影", () => { });
            //    await this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, pickupButton);
            //});

            //// SignUpWithSnsCommand
            //this.CooperationSnsCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            //this.CooperationSnsCommand.Subscribe(async () =>
            //{
            //    var buttons = new[] {
            //        ActionSheetButton.CreateCancelButton("キャンセル", () => { }),
            //        ActionSheetButton.CreateButton("Google", () => this.OpenOAuthUri(OAuthProvider.Google)),
            //        ActionSheetButton.CreateButton("Twitter", () => this.OpenOAuthUri(OAuthProvider.Twitter)),
            //        ActionSheetButton.CreateButton("LINE", () => this.OpenOAuthUri(OAuthProvider.Line))
            //    };

            //    await this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", buttons);
            //});
        }

        //private void OpenOAuthUri(OAuthProvider provider)
        //{
        //    var query = $"?user_id={this.Model.Id}&access_token={this.IsolatedStorage.AccessToken}";
        //    var url = Path.Combine(this.Environment.OAuthBaseUrl, provider.ToString().ToLower());
        //    this.SnsLoginService.Open(url + query);
        //}

        private Task CommitAsync()
        {
            var rewardedVideo = this.PageDialogService
                .DisplayAlertAsync(
                    string.Empty,
                    "動画広告を視聴して\r\n" +
                    "プロフィール画像を更新しよう！",
                    "視聴する",
                    "閉じる")
                .ToObservable(this.SchedulerProvider.UI)
                .Where(x => x)
                .SelectMany(_ => this.RewardedVideoService.DisplayRewardedVideo());

            return Observable.Return(this.Model.IconBase64)
                .SelectMany(x =>
                    x.IsNullOrEmpty()
                        ? Observable.Return(true)
                        : rewardedVideo
                )
                .SelectMany(x =>
                    this.UpdateProfileUseCase
                        .Invoke(this.Model)
                        .WithProgress(this.ProgressDialogService)
                        .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                        .Catch(Observable.Return(Unit.Default))
                    )
                .ToTask();
        }
    }
}
