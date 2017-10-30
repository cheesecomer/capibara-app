using System;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using Newtonsoft.Json;

namespace Capibara.Net.Channels
{
    public abstract class ChannelBase<TMessage> : IChannel<TMessage>, IDisposable
    {
        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler<TMessage> MessageReceive;

        protected ChannelCable Cable { get; private set; }

        public bool IsOpen => this.Cable?.IsOpen ?? false;
        
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
        public ISecureIsolatedStorage SecureIsolatedStorage { get; set; }

        protected abstract IChannelIdentifier ChannelIdentifier { get; }

        public void Dispose()
        {
            this.Cable?.Dispose();
            this.Cable = null;
        }

        public Task<bool> Connect()
        {
            this.Cable = new ChannelCable().BuildUp(this.Container);
            this.Cable.Connected += this.OnConnected;
            this.Cable.Disconnected += this.OnDisconnected;
            this.Cable.MessageReceived += this.OnMessageReceive;
            return this.Cable.Connect();
        }

        public async Task Close()
        {
            if (this.Cable != null)
                await this.Cable.Close();
        }

        private void OnConnected(object sender, EventArgs args)
        {
            this.Cable.SendSubscribe(this.ChannelIdentifier).ContinueWith(_ =>  { });

            this.Connected?.Invoke(this, null);
        }

        private void OnDisconnected(object sender, EventArgs args)
        {
            using (this.Cable)
            {
                this.Cable = null;
            }

            this.Disconnected?.Invoke(this, null);
        }

        private void OnMessageReceive(object sender,  string message)
        {
            this.MessageReceive?.Invoke(this, JsonConvert.DeserializeObject<TMessage>(message));
        }
    }
}
