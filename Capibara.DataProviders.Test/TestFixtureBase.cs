using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Capibara.Services;
using Capibara.Net;
using Capibara.Net.Channels;

using Moq;
using Unity;
using NUnit.Framework;

namespace Capibara.Test
{
    public class ReceiveMessage
    {
        public TaskCompletionSource<bool> TaskCompletionSource { get; }

        public bool IsEnd => position >= messageBuffer.Length;

        private byte[] messageBuffer;

        private int position = 0;

        private WebSocketMessageType webSocketMessageType;

        public ReceiveMessage(WebSocketMessageType webSocketMessageType, string message)
        {
            this.messageBuffer = Encoding.UTF8.GetBytes(message);
            this.TaskCompletionSource = new TaskCompletionSource<bool>();
            this.webSocketMessageType = webSocketMessageType;
        }

        public WebSocketReceiveResult Write(ArraySegment<byte> buffer)
        {
            var count =
                (position + buffer.Count >= messageBuffer.Length)
                    ? messageBuffer.Length - position
                    : buffer.Count;
            Enumerable.Range(0, count).ForEach(i => buffer.Array[i] = messageBuffer[position + i]);
            position += buffer.Count;
            var endOfMessage = position >= messageBuffer.Length;
            return new WebSocketReceiveResult(count, this.webSocketMessageType, endOfMessage);
        }
    }

    public abstract class TestFixtureBase
    {
        protected bool IsWebSocketConnectCalled { get; private set; }

        protected virtual WebSocketState WebSocketState { get; set; } = WebSocketState.Open;

        protected string WebSocketRequestUrl { get;  private set; } = string.Empty;

        protected Dictionary<string, string> WebSocketRequestHeaders { get; private set; }  = new Dictionary<string, string>();

        protected virtual List<ReceiveMessage> OptionalReceiveMessages { get; } = new List<ReceiveMessage>();

        protected List<ReceiveMessage> ReceiveMessages { get; private set; }

        protected TaskCompletionSource<HttpResponseMessage> RestSendAsyncSource { get; private set; }

        protected TaskCompletionSource<string> SendAsyncSource { get; private set; }

        protected TaskCompletionSource<bool> DisposeTaskSource { get; private set; }

        protected TaskCompletionSource<bool> ConnectTaskSource { get; private set; }

        protected TaskCompletionSource<WebSocketReceiveResult> ReceiveAsyncInfinityTaskSource { get; private set; }

        protected IEnvironment Environment { get; private set; }

        protected IIsolatedStorage IsolatedStorage { get; private set; }

        protected HttpRequestMessage RequestMessage { get; private set; }

        protected virtual string HttpStabResponse { get; } = string.Empty;

        protected virtual HttpStatusCode HttpStabStatusCode { get; } = HttpStatusCode.OK;

        protected virtual bool IsInfiniteWait { get; }

        protected virtual Exception RestException { get; }

        private Mock<IWebSocketClient> webSocketClient;

        protected Mock<IRequestFactory> RequestFactory { get; private set; }

        protected IUnityContainer Container { get; private set; }

        protected bool IsExitCalled { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            this.Dispose();

            // Environment のセットアップ
            var environment = new Mock<IEnvironment>();
            environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/api");
            environment.SetupGet(x => x.WebSocketUrl).Returns("http://localhost:9999/cable/");
            environment.SetupGet(x => x.OAuthBaseUrl).Returns("http://localhost:9999/api/oauth");
            environment.SetupGet(x => x.PrivacyPolicyUrl).Returns("http://localhost:9999/privacy_policy");
            environment.SetupGet(x => x.TermsUrl).Returns("http://localhost:9999/terms");
            environment.SetupGet(x => x.WebSocketReceiveBufferSize).Returns(1024);
            environment.SetupGet(x => x.WebSocketSendBufferSize).Returns(1024);

            // RestClient のセットアップ
            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>(), It.IsAny<IApplicationService>()));
            restClient
                .Setup(x => x.GenerateAuthenticationHeader(It.IsAny<string>()))
                .Returns<string>(token => new AuthenticationHeaderValue("Token", token));

            if (this.RestException.IsPresent())
            {
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ThrowsAsync(this.RestException);
            }
            else if (this.IsInfiniteWait)
            {
                this.RestSendAsyncSource = new TaskCompletionSource<HttpResponseMessage>();
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .Returns(() => this.RestSendAsyncSource.Task);
            }
            else
            {
                var responseMessage =
                    new HttpResponseMessage()
                    {
                        StatusCode = this.HttpStabStatusCode,
                        Content = new Net.HttpContentHandler()
                        {
                            ResultOfString = this.HttpStabResponse ?? string.Empty
                        }
                    };
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync((HttpRequestMessage request) => {
                        this.RequestMessage = request;
                        return responseMessage;
                    });
            }

            // ISecureIsolatedStorage のセットアップ
            var isolatedStorage = new Mock<IIsolatedStorage>();
            isolatedStorage.SetupAllProperties();

            var clientWebSocketOptions = new Mock<IWebSocketOptions>();
            clientWebSocketOptions.SetupAllProperties();
            clientWebSocketOptions
                .Setup(x => x.SetRequestHeader(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => WebSocketRequestHeaders[k] = v);

            // IWebSocketClient のセットアップ
            webSocketClient = new Mock<IWebSocketClient>();
            webSocketClient.SetupAllProperties();
            webSocketClient.SetupGet(x => x.Options).Returns(clientWebSocketOptions.Object);
            webSocketClient.SetupGet(x => x.State).Returns(() => this.WebSocketState);

            this.ResetDispose();

            this.ConnectTaskSource = new TaskCompletionSource<bool>();
            webSocketClient
                .Setup(x => x.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns(() => ConnectTaskSource.Task)
                .Callback((Uri uri, CancellationToken token) =>
                {
                    this.WebSocketRequestUrl = uri.AbsoluteUri;
                    ConnectTaskSource.TrySetResult(true);
                    this.IsWebSocketConnectCalled = true;
                });

            this.ReceiveMessages = new List<ReceiveMessage>() {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"welcome\"}")
            }.Concat(this.OptionalReceiveMessages).ToList();

            var receiveMessageQueue = new Queue<ReceiveMessage>();
            ReceiveMessages.ForEach(x => receiveMessageQueue.Enqueue(x));

            webSocketClient
                .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Returns((ArraySegment<byte> buffer, CancellationToken cancellationToken) =>
                {
                    if (receiveMessageQueue.Peek().IsEnd)
                    {
                        receiveMessageQueue.Peek().TaskCompletionSource.TrySetResult(true);
                        receiveMessageQueue.Dequeue();
                    }

                    if (receiveMessageQueue.Count == 0)
                    {
                        this.ReceiveAsyncInfinityTaskSource = new TaskCompletionSource<WebSocketReceiveResult>();
                        return this.ReceiveAsyncInfinityTaskSource.Task;
                    }

                    return Task.Run(() => receiveMessageQueue.Peek().Write(buffer));
                });

            this.ResetSendAsync();

            // IWebSocketClientFactory のセットアップ
            var webSocketClientFactory = new Mock<IWebSocketClientFactory>();
            webSocketClientFactory.Setup(x => x.Create()).Returns(webSocketClient.Object);

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

            this.RequestFactory = new Mock<IRequestFactory>();

            var applicationService = new Mock<IApplicationService>();
            applicationService.Setup(x => x.Exit()).Callback(() => this.IsExitCalled = true);
            applicationService.SetupGet(x => x.StoreUrl).Returns("http://example.com/store");
            applicationService.SetupGet(x => x.AppVersion).Returns("1.0.0");
            applicationService.SetupGet(x => x.Platform).Returns("iOS");

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IEnvironment>(this.Environment = environment.Object);
            container.RegisterInstance<IRestClient>(restClient.Object);
            container.RegisterInstance<IIsolatedStorage>(this.IsolatedStorage = isolatedStorage.Object);
            container.RegisterInstance<ICapibaraApplication>(application.Object);
            container.RegisterInstance<IWebSocketClientFactory>(webSocketClientFactory.Object);
            container.RegisterInstance<IRequestFactory>(this.RequestFactory.Object);
            container.RegisterInstance(applicationService.Object);

            this.Container = container;
        }

        [TearDown]
        public void Dispose()
        {
            this.ConnectTaskSource?.TrySetCanceled();
            this.ConnectTaskSource?.Task?.Dispose();
            this.ConnectTaskSource = null;

            this.DisposeTaskSource?.TrySetCanceled();
            this.DisposeTaskSource?.Task?.Dispose();
            this.DisposeTaskSource = null;

            this.SendAsyncSource?.TrySetCanceled();
            this.SendAsyncSource?.Task?.Dispose();
            this.SendAsyncSource = null;

            this.ReceiveMessages?.ForEach(x => {
                x.TaskCompletionSource?.TrySetCanceled();
                x.TaskCompletionSource?.Task?.Dispose();
            });
            this.ReceiveMessages?.Clear();

            this.ReceiveAsyncInfinityTaskSource?.TrySetCanceled();
            this.ReceiveAsyncInfinityTaskSource?.Task?.Dispose();
            this.ReceiveAsyncInfinityTaskSource = null;

            this.RestSendAsyncSource?.TrySetCanceled();
            this.RestSendAsyncSource?.Task?.Dispose();
            this.RestSendAsyncSource = null;
        }

        public void ResetDispose()
        {
            this.DisposeTaskSource?.TrySetCanceled();
            this.DisposeTaskSource?.Task?.Dispose();
            this.DisposeTaskSource = new TaskCompletionSource<bool>();
            webSocketClient
                .Setup(x => x.Dispose())
                .Callback(() => DisposeTaskSource.TrySetResult(true));
        }

        public void ResetSendAsync()
        {
            this.SendAsyncSource?.TrySetCanceled();
            this.SendAsyncSource?.Task?.Dispose();
            this.SendAsyncSource = new TaskCompletionSource<string>();
            var isTextMessage = It.Is<WebSocketMessageType>(v => v == WebSocketMessageType.Text);
            var stream = new MemoryStream();
            webSocketClient.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), isTextMessage, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) =>
                {
                    stream.Write(buffer.Array, buffer.Offset, buffer.Count());

                    if (endOfMessage)
                        using (stream)
                        {
                            stream.Position = 0;
                            using (var reader = new StreamReader(stream))
                            SendAsyncSource.TrySetResult(reader.ReadToEnd());
                        }

                    return Task.Run(() => { });
                });
        }
    }

    public abstract class ChannelTestFixtureBase<TMessage> : TestFixtureBase
    {
        protected virtual bool HasEventHandler { get; }

        protected abstract IChannel<TMessage> Channel { get; }

        protected bool IsFireMessageReceive { get; private set; }

        protected bool IsFireConnected { get; private set; }

        protected bool IsFireDisconnected { get; private set; }

        protected virtual bool NeedConnect { get; } = true;

        protected virtual bool NeedWaitConnect { get; } = true;

        protected virtual bool NeedWaitReceiveMessage { get; } = true;

        protected virtual bool NeedWaitSendMessage { get; } = true;

        protected virtual bool NeedWaitDispose { get; } = false;

        protected string SendMessage { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Channel.BuildUp(this.Container);

            if (this.HasEventHandler)
            {
                this.Channel.Connected += (sender, e) => this.IsFireConnected = true;
                this.Channel.Disconnected += (sender, e) => this.IsFireDisconnected = true;
                this.Channel.MessageReceive += (sender, e) => this.IsFireMessageReceive = true;
            }

            if (this.NeedConnect)
            {
                this.Channel.Connect();

                // 接続処理終了を待機
                if (this.NeedWaitConnect)
                    this.ConnectTaskSource.Task.Wait();

                // 受信完了を待機
                if (this.NeedWaitReceiveMessage)
                    Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());

                // 送信完了を待機
                if (this.NeedWaitSendMessage)
                {
                    this.SendAsyncSource.Task.Wait();

                    this.SendMessage = this.SendAsyncSource.Task.Result;
                }

                if (this.NeedWaitDispose)
                    this.DisposeTaskSource.Task.Wait();
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.Channel.Dispose();
        }
    }
}
