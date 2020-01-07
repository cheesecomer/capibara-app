using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity;

namespace Capibara.Net.Channels
{
    public abstract class ChannelBase<TMessage> : IDisposable
    {
        public virtual event EventHandler Connected;

        public virtual event EventHandler Disconnected;

        public virtual event EventHandler RejectSubscription;

        public virtual event EventHandler<EventArgs<TMessage>> MessageReceive;

        protected ChannelCableBase Cable { get; private set; }

        public virtual bool IsOpen => this.Cable?.IsOpen ?? false;

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

        [Dependency]
        public IChannelCableFactory ChannelCableFactory { get; set; }

        public abstract IChannelIdentifier ChannelIdentifier { get; }

        public virtual void Dispose()
        {
            this.Cable?.Dispose();
            this.Cable = null;
        }

        public virtual Task<bool> Connect()
        {
            this.Cable = this.ChannelCableFactory.Create().BuildUp(this.Container);
            this.Cable.Connected += this.OnConnected;
            this.Cable.Disconnected += this.OnDisconnected;
            this.Cable.MessageReceived += this.OnMessageReceive;
            this.Cable.RejectSubscriptionReceived += this.OnRejectSubscriptionReceived;
            return this.Cable.Connect();
        }

        public virtual async Task Close()
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
            this.Cable?.Dispose();
            this.Cable = null;

            this.Disconnected?.Invoke(this, null);
        }

        private void OnRejectSubscriptionReceived(object sender, EventArgs args)
        {
            this.RejectSubscription?.Invoke(this, null);
        }

        private void OnMessageReceive(object sender,  EventArgs<string> args)
        {
            this.MessageReceive?.Invoke(this, JsonConvert.DeserializeObject<TMessage>(args.Value));
        }
    }
}
