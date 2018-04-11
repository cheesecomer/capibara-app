using System;
namespace Capibara
{
    public interface IEnvironment
    {
        /// <summary>
        /// 基底のURLを取得します
        /// </summary>
        /// <value>The base URL.</value>
        string BaseUrl { get; }

        /// <summary>
        /// APIの基底URLを取得します
        /// </summary>
        string ApiBaseUrl { get; }

        /// <summary>
        /// WEBソケット接続先URLを取得します
        /// </summary>
        string WebSocketUrl { get; }

        /// <summary>
        /// OAuth の基底URLを取得します
        /// </summary>
        /// <value>The base URL.</value>
        string OAuthBaseUrl { get; }

        /// <summary>
        /// 利用規約のURLを取得します。
        /// </summary>
        /// <value>The terms URL.</value>
        string TermsUrl { get; }

        /// <summary>
        /// プライバシーポリシーのURLを取得します。
        /// </summary>
        /// <value>The privacy policy URL.</value>
        string PrivacyPolicyUrl { get; }

        /// <summary>
        /// Webソケット受信バッファサイズを取得します
        /// </summary>
        int WebSocketReceiveBufferSize { get; }

        /// <summary>
        /// Webソケット送信バッファサイズを取得します
        /// </summary>
        int WebSocketSendBufferSize { get; }
    }
}
