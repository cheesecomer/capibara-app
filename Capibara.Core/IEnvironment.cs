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
        /// Webソケット受信バッファサイズを取得します
        /// </summary>
        int WebSocketReceiveBufferSize { get; }

        /// <summary>
        /// Webソケット送信バッファサイズを取得します
        /// </summary>
        int WebSocketSendBufferSize { get; }
    }
}
