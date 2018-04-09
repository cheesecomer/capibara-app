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
        protected virtual List<ReceiveMessage> OptionalReceiveMessages { get; } = new List<ReceiveMessage>();

        protected List<ReceiveMessage> ReceiveMessages { get; private set; }

        protected TaskCompletionSource<string> SendAsyncSource { get; private set; }

        protected TaskCompletionSource<bool> ConnectTaskSource { get; private set; }

        protected TaskCompletionSource<WebSocketReceiveResult> ReceiveAsyncInfinityTaskSource { get; private set; }

        protected IEnvironment Environment { get; private set; }

        protected IIsolatedStorage IsolatedStorage { get; private set; }

        protected HttpRequestMessage RequestMessage { get; private set; }

        protected virtual string HttpStabResponse { get; } = string.Empty;

        protected virtual HttpStatusCode HttpStabStatusCode { get; } = HttpStatusCode.OK;

        protected Mock<IRestClient> RestClient { get; private set; }

        protected Mock<IWebSocketOptions> WebSocketOptions { get; private set; }

        protected Mock<IWebSocketClient> WebSocketClient { get; private set; }

        protected Mock<IRequestFactory> RequestFactory { get; private set; }

        protected Mock<IChannelFactory> ChannelFactory { get; private set; }

        protected Mock<IChannelCableFactory> ChannelCableFactory { get; private set; }

        protected Mock<IApplicationService> ApplicationService { get; private set; }

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
            this.RestClient = new Mock<IRestClient>();
            this.RestClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>(), It.IsAny<IApplicationService>()));
            this.RestClient
                .Setup(x => x.GenerateAuthenticationHeader(It.IsAny<string>()))
                .Returns<string>(token => new AuthenticationHeaderValue("Token", token));

            var responseMessage =
                new HttpResponseMessage()
                {
                    StatusCode = this.HttpStabStatusCode,
                    Content = new Net.HttpContentHandler()
                    {
                        ResultOfString = this.HttpStabResponse ?? string.Empty
                    }
                };
            this.RestClient
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync((HttpRequestMessage request) => {
                    this.RequestMessage = request;
                    return responseMessage;
                });

            // ISecureIsolatedStorage のセットアップ
            var isolatedStorage = new Mock<IIsolatedStorage>();
            isolatedStorage.SetupAllProperties();

            this.WebSocketOptions = new Mock<IWebSocketOptions>();
            this.WebSocketOptions.SetupAllProperties();
            this.WebSocketOptions
                .Setup(x => x.SetRequestHeader(It.IsAny<string>(), It.IsAny<string>()));

            // IWebSocketClient のセットアップ
            this.WebSocketClient = new Mock<IWebSocketClient>();
            this.WebSocketClient.SetupAllProperties();
            this.WebSocketClient.SetupGet(x => x.Options).Returns(this.WebSocketOptions.Object);

            //this.ResetDispose();

            this.ConnectTaskSource = new TaskCompletionSource<bool>();
            this.WebSocketClient
                .Setup(x => x.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            this.ReceiveMessages = new List<ReceiveMessage>
            {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"welcome\"}")
            }.Concat(this.OptionalReceiveMessages).ToList();

            var receiveMessageQueue = new Queue<ReceiveMessage>();
            ReceiveMessages.ForEach(x => receiveMessageQueue.Enqueue(x));

            this.WebSocketClient
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
            webSocketClientFactory.Setup(x => x.Create()).Returns(WebSocketClient.Object);

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

            this.RequestFactory = new Mock<IRequestFactory>();

            this.ChannelFactory = new Mock<IChannelFactory>();

            this.ChannelCableFactory = new Mock<IChannelCableFactory>();

            this.ApplicationService = new Mock<IApplicationService>();
            this.ApplicationService.Setup(x => x.Exit());
            this.ApplicationService.SetupGet(x => x.StoreUrl).Returns("http://example.com/store");
            this.ApplicationService.SetupGet(x => x.AppVersion).Returns("1.0.0");
            this.ApplicationService.SetupGet(x => x.Platform).Returns("iOS");

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance(this.Environment = environment.Object);
            container.RegisterInstance(this.RestClient.Object);
            container.RegisterInstance(this.IsolatedStorage = isolatedStorage.Object);
            container.RegisterInstance(application.Object);
            container.RegisterInstance(webSocketClientFactory.Object);
            container.RegisterInstance(this.RequestFactory.Object);
            container.RegisterInstance(this.ChannelFactory.Object);
            container.RegisterInstance(this.ChannelCableFactory.Object);
            container.RegisterInstance(this.ApplicationService.Object);

            this.Container = container;
        }

        [TearDown]
        public void Dispose()
        {
            this.ConnectTaskSource?.TrySetCanceled();
            this.ConnectTaskSource?.Task?.Dispose();
            this.ConnectTaskSource = null;

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
        }

        public void ResetSendAsync()
        {
            this.SendAsyncSource?.TrySetCanceled();
            this.SendAsyncSource?.Task?.Dispose();
            this.SendAsyncSource = new TaskCompletionSource<string>();
            var isTextMessage = It.Is<WebSocketMessageType>(v => v == WebSocketMessageType.Text);
            var stream = new MemoryStream();
            WebSocketClient.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), isTextMessage, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
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
}
