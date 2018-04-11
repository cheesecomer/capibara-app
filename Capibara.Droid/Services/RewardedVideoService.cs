using System;
using System.Threading.Tasks;

using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;

using Capibara.Services;

namespace Capibara.Droid.Services
{
    public class RewardedVideoService : IRewardedVideoService
    {
        Task<bool> IRewardedVideoService.DisplayRewardedVideo()
        {
            var taskSource = new TaskCompletionSource<bool>();

            var rewardedVideoAd = MobileAds.GetRewardedVideoAdInstance(MainActivity.Instance);

            rewardedVideoAd.RewardedVideoAdListener =
                new RewardedVideoAdListener(rewardedVideoAd, taskSource);

            rewardedVideoAd.LoadAd(
                PlatformVariable.AdMobUnitIdForRewardedVideo,
                new AdRequest.Builder().Build());

            return taskSource.Task;
        }

        public class RewardedVideoAdListener : Java.Lang.Object, IRewardedVideoAdListener
        {
            private bool result = false;

            private IRewardedVideoAd rewardedVideoAd;

            private TaskCompletionSource<bool> taskSource;

            public RewardedVideoAdListener(IRewardedVideoAd rewardedVideoAd, TaskCompletionSource<bool> taskSource)
            {
                this.rewardedVideoAd = rewardedVideoAd;
                this.taskSource = taskSource;
            }

            public void OnRewarded(IRewardItem reward)
            {
                this.result = true;
            }

            public void OnRewardedVideoAdClosed()
            {
                this.rewardedVideoAd?.Dispose();
                this.rewardedVideoAd = null;

                this.taskSource.TrySetResult(this.result);
            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                this.rewardedVideoAd?.Dispose();
                this.rewardedVideoAd = null;

                this.taskSource.TrySetResult(true);
            }

            public void OnRewardedVideoAdLeftApplication() { }

            public void OnRewardedVideoAdLoaded()
            {
                if (this.rewardedVideoAd.IsLoaded)
                {
                    this.rewardedVideoAd.Show();
                }
                else
                {
                    this.rewardedVideoAd?.Dispose();
                    this.rewardedVideoAd = null;
                    this.taskSource.TrySetResult(true);
                }
            }

            public void OnRewardedVideoAdOpened() { }

            public void OnRewardedVideoStarted() { }
        }
    }
}
