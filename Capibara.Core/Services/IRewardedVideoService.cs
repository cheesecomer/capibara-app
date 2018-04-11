using System;

using System.Threading.Tasks;

namespace Capibara.Services
{
    public interface IRewardedVideoService
    {
        Task<bool> DisplayRewardedVideo();
    }
}
