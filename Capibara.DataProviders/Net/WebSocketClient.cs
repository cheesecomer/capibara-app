using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Capibara.Net
{
    public interface IWebSocketOptions
    {
        TimeSpan KeepAliveInterval { get; set; }

        void SetRequestHeader(string headerName, string headerValue);
    }

    public interface IWebSocketClientFactory
    {
        IWebSocketClient Create();
    }

    public interface IWebSocketClient
    {
        WebSocketState State { get; }

        IWebSocketOptions Options { get; }

        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task SendAsync(
            ArraySegment<byte> buffer,
            WebSocketMessageType messageType,
            bool endOfMessage,
            CancellationToken cancellationToken);

        Task<WebSocketReceiveResult> ReceiveAsync(
            ArraySegment<byte> buffer,
            CancellationToken cancellationToken);

        Task CloseAsync(
            WebSocketCloseStatus closeStatus,
            string statusDescription,
            CancellationToken cancellationToken);

        void Dispose();
    }

    public class WebSocketClientFactory : IWebSocketClientFactory
    {
        IWebSocketClient IWebSocketClientFactory.Create()
            => new WebSocketClient(new ClientWebSocket());
    }

    public class WebSocketOptions : IWebSocketOptions
    {
        private readonly ClientWebSocketOptions options;

        public TimeSpan KeepAliveInterval
        {
            get => this.options.KeepAliveInterval;
            set => this.options.KeepAliveInterval = value;
        }

        public WebSocketOptions(ClientWebSocketOptions options)
        {
            this.options = options;
        }

        public void SetRequestHeader(string headerName, string headerValue)
            => this.options.SetRequestHeader(headerName, headerValue);
    }

    public class WebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket client;

        public WebSocketState State => this.client.State;

        public IWebSocketOptions Options { get; private set; }
        
        public WebSocketClient(ClientWebSocket client)
        {
            this.client = client;
            this.Options = new WebSocketOptions(client.Options);
        }

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
             => this.client.ConnectAsync(uri, cancellationToken);

        public Task CloseAsync(
            WebSocketCloseStatus closeStatus,
            string statusDescription,
            CancellationToken cancellationToken)
            => this.client.CloseAsync(closeStatus, statusDescription, cancellationToken);

        public Task SendAsync(
            ArraySegment<byte> buffer,
            WebSocketMessageType messageType,
            bool endOfMessage,
            CancellationToken cancellationToken)
             => this.client.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public Task<WebSocketReceiveResult> ReceiveAsync(
            ArraySegment<byte> buffer,
            CancellationToken cancellationToken)
             => this.client.ReceiveAsync(buffer, cancellationToken);

        public void Dispose() => this.client.Dispose();
    }
}
