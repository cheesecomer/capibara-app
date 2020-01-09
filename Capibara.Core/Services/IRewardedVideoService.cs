using System;

namespace Capibara.Services
{
    public interface IRewardedVideoService
    {
        IObservable<bool> DisplayRewardedVideo();
    }
}
