using System;
namespace Capibara.Models
{
    /// <summary>
    /// エラーモデル
    /// </summary>
    public class Error
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// 詳細エラーコード
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; set; }
    }
}
