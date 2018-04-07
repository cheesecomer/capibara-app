using System.Threading.Tasks;
using Capibara.Services;
using Foundation;
using Google.MobileAds;
using UIKit;

namespace Capibara.iOS.Services
{
    public class RewardedVideoService : IRewardedVideoService
    {
        Task<bool> IRewardedVideoService.DisplayRewardedVideo()
        {
            var taskSource = new TaskCompletionSource<bool>();

            RewardBasedVideoAd.SharedInstance.Delegate = new RewardVideoAdDelegate(taskSource);

            RewardBasedVideoAd.SharedInstance.LoadRequest(
                Request.GetDefaultRequest(), 
                PlatformVariable.AdMobUnitIdForRewardedVideo);

            return taskSource.Task;
        }

        public class RewardVideoAdDelegate : RewardBasedVideoAdDelegate
        {
            private TaskCompletionSource<bool> taskSource;

            private bool result = false;

            public RewardVideoAdDelegate(TaskCompletionSource<bool> taskSource)
            {
                this.taskSource = taskSource;
            }

            public override void DidRewardUser(RewardBasedVideoAd rewardBasedVideoAd, AdReward reward)
            {
                this.result = true;
            }

            public override void DidClose(RewardBasedVideoAd rewardBasedVideoAd)
            {
                this.taskSource.TrySetResult(this.result);
            }

            public override void DidReceiveAd(RewardBasedVideoAd rewardBasedVideoAd)
            {
                if (rewardBasedVideoAd.IsReady)
                {
                    var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                    while (viewController.PresentedViewController != null)
                    {
                        viewController = viewController.PresentedViewController;
                    }

                    rewardBasedVideoAd.PresentFromRootViewController(viewController);
                }
                else
                {
                    this.taskSource.TrySetResult(true);
                }
            }

            public override void DidFailToLoad(RewardBasedVideoAd rewardBasedVideoAd, NSError error)
            {
                this.taskSource.TrySetResult(true);
            }
        }
    }
}
