using Capibara.Attributes;

namespace Capibara.Domain.Models
{
    /// <summary>
    /// 通報理由
    /// </summary>
    public enum ReportReason
    {
        /// <summary>
        /// その他
        /// </summary>
        [DisplayName("その他")]
        Other = 0,

        /// <summary>
        /// スパム
        /// </summary>
        [DisplayName("スパム")]
        Spam = 1,

        /// <summary>
        /// 暴力的もしくは差別的な発言
        /// </summary>
        [DisplayName("暴力的もしくは差別的な発言")]
        AbusiveOrHatefulSpeech = 2,

        /// <summary>
        /// 暴力的もしくは差別的な画像
        /// </summary>
        [DisplayName("暴力的もしくは差別的な画像")]
        AbusiveOrHatefulImage = 3,

        /// <summary>
        /// 卑猥な発言
        /// </summary>
        [DisplayName("卑猥な発言")]
        ObsceneSpeech = 4,

        /// <summary>
        /// 卑猥な画像
        /// </summary>
        [DisplayName("卑猥な画像")]
        ObsceneImage = 5,
    }
}
