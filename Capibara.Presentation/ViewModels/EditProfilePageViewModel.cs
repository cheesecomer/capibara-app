using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class EditProfilePageViewModel : UserViewModel
    {
        [Unity.Dependency]
        public IUpdateProfileUseCase UpdateProfileUseCase { get; set; }

        [Unity.Dependency]
        public IPickupPhotoFromAlbumUseCase PickupPhotoFromAlbumUseCase { get; set; }

        [Unity.Dependency]
        public IOAuthCooperationUseCase OAuthCooperationUseCase { get; set; }

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
            this.CommitCommand =
                this.Nickname
                    .Select(x => x.ToSlim().IsPresent())
                    .ToAsyncReactiveCommand()
                    .WithSubscribe(this.CommitAsync)
                    .AddTo(this.Disposable);

            // ChangePhotoCommand
            this.ChangePhotoCommand =
                new AsyncReactiveCommand()
                    .WithSubscribe(this.ChangePhotoAsync)
                    .AddTo(this.Disposable);

            var actionSheetButtons = new[] {
                ActionSheetButton.CreateCancelButton("キャンセル", () => { }),
                ActionSheetButton.CreateButton("Google", () => this.OAuthCooperationUseCase.Invoke(OAuthProvider.Google).Wait()),
                ActionSheetButton.CreateButton("Twitter", () => this.OAuthCooperationUseCase.Invoke(OAuthProvider.Twitter).Wait()),
                ActionSheetButton.CreateButton("LINE", () => this.OAuthCooperationUseCase.Invoke(OAuthProvider.Line).Wait())
            };

            // CooperationSnsCommand
            this.CooperationSnsCommand = new AsyncReactiveCommand()
                .WithSubscribe(() => this.PageDialogService.DisplayActionSheetAsync("SNSでログイン", actionSheetButtons))
                .AddTo(this.Disposable);
        }

        //private void OpenOAuthUri(OAuthProvider provider)
        //{
        //    var query = $"?user_id={this.Model.Id}&access_token={this.IsolatedStorage.AccessToken}";
        //    var url = Path.Combine(this.Environment.OAuthBaseUrl, provider.ToString().ToLower());
        //    this.SnsLoginService.Open(url + query);
        //}

        private Task CommitAsync()
        {
            return Observable.Defer(() =>
                    this.Model.IconBase64.IsNullOrEmpty()
                        ? Observable.Return(true)
                        : Observable.FromAsync(
                            () => this.PageDialogService
                                .DisplayAlertAsync(
                                    string.Empty,
                                    "動画広告を視聴して\r\n" +
                                    "プロフィール画像を更新しよう！",
                                    "視聴する",
                                    "閉じる"),
                            this.SchedulerProvider.UI)
                            .Where(v => v)
                            .SelectMany(
                                _ => this.RewardedVideoService.DisplayRewardedVideo())
                            .SubscribeOn(this.SchedulerProvider.UI))
                .Where(v => v)
                .SubscribeOn(this.SchedulerProvider.UI)
                .SelectMany(
                    x => Observable
                            .Defer(() => this.UpdateProfileUseCase.Invoke(this.Model))
                            .SubscribeOn(this.SchedulerProvider.IO)
                            .WithProgress(this.ProgressDialogService)
                            .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                            .Select(_ => true)
                            .Catch(Observable.Return(false)))
                .Where(v => v)
                .SelectMany(
                    x => this.NavigationService
                            .GoBackAsync(new NavigationParameters { { ParameterNames.Model, this.Model } })
                            .ToObservable()
                            .Select(_ => Unit.Default))
                .SubscribeOn(this.SchedulerProvider.UI)
                // 値を返さない Task を AsyncReactiveCommand に設定すると死ぬ。
                .DefaultIfEmpty(Unit.Default)
                .ToTask();
        }

        private Task ChangePhotoAsync()
        {
            var cancelButton = ActionSheetButton.CreateCancelButton("キャンセル", () => { });

#pragma warning disable RECS0165 // 非同期メソッドは void ではなくタスクを返す必要があります
            var pickupButton = ActionSheetButton.CreateButton("アルバムから選択", async () => await PickupPhotoAsync());
#pragma warning restore RECS0165 // 非同期メソッドは void ではなくタスクを返す必要があります


            return Observable
                .FromAsync(
                    () => this.PageDialogService.DisplayActionSheetAsync("プロフィール画像変更", cancelButton, pickupButton),
                    this.SchedulerProvider.UI)
                .ToTask();
        }

        private Task PickupPhotoAsync()
        {
            return Observable
                .Defer(this.PickupPhotoFromAlbumUseCase.Invoke)
                .SelectMany(x => Observable.Create<Unit>(observer =>
                {
                    this.Model.IconBase64 = x;
                    observer.OnCompleted();
                    return () => { };
                }))
                .SubscribeOn(this.SchedulerProvider.UI)
                .DefaultIfEmpty(Unit.Default)
                .ToTask()
                ;
        }
    }
}
