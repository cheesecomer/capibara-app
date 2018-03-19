using System;
namespace Capibara.Models
{
    /// <summary>
    /// 通報理由
    /// </summary>
    public enum ReportReason
    {
        /// <summary>
        /// その他
        /// </summary>
        Other = 0,

        /// <summary>
        /// スパム
        /// </summary>
        Spam = 1,

        /// <summary>
        /// 暴力的もしくは差別的な発言
        /// </summary>
        AbusiveOrHatefulSpeech = 2,

        /// <summary>
        /// 暴力的もしくは差別的な画像
        /// </summary>
        AbusiveOrHatefulImage = 3,

        /// <summary>
        /// 卑猥な発言
        /// </summary>
        ObsceneSpeech = 4,

        /// <summary>
        /// 卑猥な画像
        /// </summary>
        ObsceneImage = 5,
    }
}
