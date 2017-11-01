using System;
using System.IO;
using System.Collections.Generic;

using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace Capibara.Net.Channels
{
    public enum MessageType
    {
        Welcome,
        Ping,
        ConfirmSubscription,
        Message,
        Unknown
    }

    public class ChannelCable : IDisposable
    {
        private IWebSocketClient webSocket;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler PingReceived;

        public event EventHandler ConfirmSubscriptionReceived;

        public event EventHandler<string> MessageReceived;

        public event EventHandler MessageSent;

        public bool IsOpen
            => (this.webSocket?.State ?? WebSocketState.None) == WebSocketState.Open;

        public Exception LastException;

        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [Dependency]
        public IUnityContainer Container { get; set; }

        /// <summary>
        /// 環境設定
        /// </summary>
        /// <value>The environment.</value>
        [Dependency]
        public IEnvironment Environment { get; set; }

        /// <summary>
        /// セキュア分離ストレージ
        /// </summary>
        /// <value>The secure isolated storage.</value>
        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        /// <summary>
        /// セキュア分離ストレージ
        /// </summary>
        /// <value>The secure isolated storage.</value>
        [Dependency]
        public IWebSocketClientFactory WebSocketClientFactory{ get; set; }

        public void Dispose()
        {
            this.webSocket?.Dispose();
            this.webSocket = null;
        }

        /// <summary>
        /// チャンネルに接続します。
        /// </summary>
        public Task<bool> Connect()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var url = new Uri(this.Environment.WebSocketUrl);
            this.webSocket = this.WebSocketClientFactory.Create();

            this.webSocket.Options.SetRequestHeader("Authorization", $"Token {this.IsolatedStorage.AccessToken}");
            this.webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

            this.webSocket
                .ConnectAsync(url, CancellationToken.None)
                .ContinueWith(
                    scheduler: TaskScheduler.FromCurrentSynchronizationContext(),
                    continuationFunction: async _ =>
                    {
                        var isOpen = this.webSocket.State == WebSocketState.Open;
                        if (!isOpen)
                        {
                            taskCompletionSource.SetResult(false);
                            this.Disconnected?.Invoke(this, null);
                            this.Dispose();
                            return;
                        }

                        try
                        {
                            await this.Listen(taskCompletionSource);
                        }
                        catch (Exception e)
                        {
                            taskCompletionSource.TrySetException(e);
                            this.LastException = e;
                        }

                        if (this.webSocket != null)
                        {
                            this.Disconnected?.Invoke(this, null);
                            this.Dispose();
                        }
                    });

            return taskCompletionSource.Task;
        }

        public async Task Close()
        {
            if (!this.IsOpen)
                return;
                    
            await this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            this.Disconnected?.Invoke(this, null);
            this.Dispose();
        }

        public Task SendSubscribe(IChannelIdentifier channelIdentifier)
        {
            var command = new SubscribeCommand(channelIdentifier);
            return this.SendCommand(command);
        }

        public async Task SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var chunkSize = this.Environment.WebSocketSendBufferSize;
            var message = JsonConvert.SerializeObject(command);
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = messageBuffer.Length / chunkSize + (messageBuffer.Length % chunkSize == 0 ? 0 : 1);
            for (var i = 0; i < messagesCount && this.IsOpen; i++)
            {
                var isLastMessage = (i + 1) == messagesCount;
                var offset = chunkSize * i;
                var count = isLastMessage ? messageBuffer.Length - offset : chunkSize;
                var buffer = new ArraySegment<byte>(messageBuffer, offset, count);
                await this.webSocket.SendAsync(buffer, WebSocketMessageType.Text, isLastMessage, CancellationToken.None);
            }

            this.MessageSent?.Invoke(this, null);
        }

        private async Task Listen(TaskCompletionSource<bool> connectTaskCompletionSource)
        {
            bool isClosed = false;
            while (this.IsOpen && !isClosed)
            {
                WebSocketReceiveResult result;
                using (var stream = new MemoryStream())
                using (var reader = new StreamReader(stream))
                {
                    do
                    {
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[this.Environment.WebSocketReceiveBufferSize]);
                        result = await this.webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        if (result.MessageType.IsClose())
                        {
                            isClosed = true;
                        }
                        else
                        {
                            stream.Write(buffer.Array, 0, result.Count);
                        }
                    }
                    while (!result.EndOfMessage && this.IsOpen && !isClosed);

                    stream.Position = 0;

                    if (this.IsOpen && !isClosed && result.MessageType.IsText())
                    {
                        var messageType = this.OnMessageReceive(reader.ReadToEnd());
                        if (messageType == MessageType.Welcome)
                        {
                            connectTaskCompletionSource.SetResult(true);
                        }
                    }
                }
            }

            connectTaskCompletionSource.TrySetResult(false);
        }

        private MessageType OnMessageReceive(string message)
        {
            Dictionary<string, object> json;
            MessageType result = MessageType.Unknown;
            try
            {
                json =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            }
            catch
            {
                json = null;
            }

            var type = json?.ValueOrDefault("type") as string;
            var hasMessage = json?.ValueOrDefault("message").IsPresent() ?? false;
            if (type.IsNullOrEmpty() && hasMessage)
            {
                this.MessageReceived?.Invoke(this, json["message"].ToString());
                result = MessageType.Message;
            }
            else if (type == "welcome")
            {
                this.Connected?.Invoke(this, null);
                result = MessageType.Welcome;
            }
            else if (type == "ping")
            {
                this.PingReceived?.Invoke(this, null);
                result = MessageType.Ping;
            }
            else if (type == "confirm_subscription")
            {
                this.ConfirmSubscriptionReceived?.Invoke(this, null);
                result = MessageType.ConfirmSubscription;
            }

            return result;
        }
    }
}
