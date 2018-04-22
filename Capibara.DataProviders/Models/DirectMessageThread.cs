using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Net.Channels;

using Newtonsoft.Json;

namespace Capibara.Models
{
    public class DirectMessageThread : ModelBase<DirectMessageThread>
    {
        private bool isConnected;

        private DirectMessage latestDirectMessage = new DirectMessage();

        private User user = new User();

        private DirectMessageChannelBase channel;

        public virtual event EventHandler RefreshSuccess;

        public virtual event EventHandler<FailEventArgs> RefreshFail;

        public virtual event EventHandler SpeakSuccess;

        public virtual event EventHandler<FailEventArgs> SpeakFail;

        public virtual event EventHandler Disconnected;

        [JsonProperty("latest_direct_message")]
        public DirectMessage LatestDirectMessage
        {
            get => this.latestDirectMessage;
            set => this.SetProperty(ref this.latestDirectMessage, value);
        }

        public User User
        {
            get => this.user;
            set => this.SetProperty(ref this.user, value);
        }

        public virtual bool IsConnected
        {
            get => this.isConnected;
            set => this.SetProperty(ref this.isConnected, value);
        }

        public ObservableCollection<DirectMessage> DirectMessages { get; }
            = new ObservableCollection<DirectMessage>();

        public override void Restore(DirectMessageThread model)
        {
            this.User.Restore(model.User);
            this.LatestDirectMessage.Restore(model.LatestDirectMessage);
        }

        public virtual async Task<bool> Refresh()
        {
            var request = this.RequestFactory.DirectMessagesShowRequest(this.User).BuildUp(this.Container);
            try
            {
                var result = await request.Execute();

                result.DirectMessages?
                      .Where(x => this.DirectMessages.All(y => y.Id != x.Id))?
                      .ForEach(x => this.DirectMessages.Insert(0, x.BuildUp(this.Container)));

                this.RefreshSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.RefreshFail?.Invoke(this, e);

                return false;
            }
        }

        public virtual async Task<bool> Connect()
        {
            if (this.channel != null && this.channel.IsOpen)
            {
                return true;
            }

            if (this.channel != null)
            {
                await this.channel.Close();
            }

            this.channel = this.ChannelFactory.CreateDirectMessageChannel(this.User).BuildUp(this.Container);

            this.channel.Connected += (sender, e) => this.IsConnected = true;
            this.channel.MessageReceive += this.OnMessageReceive;
            this.channel.Disconnected += async (sender, e) =>
            {
                await this.Close();
                this.Disconnected?.Invoke(this, null);
            };

            return await this.channel.Connect();
        }

        public virtual async Task<bool> Close()
        {
            this.IsConnected = false;
            if (this.channel != null)
            {
                await this.channel?.Close();
                this.channel?.Dispose();
            }

            this.channel = null;
            return true;
        }

        public virtual async Task<bool> Speak(string message)
        {
            try
            {
                await this.channel.Speak(message);

                this.SpeakSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.SpeakFail?.Invoke(this, e);

                return false;
            }
        }

        private void OnMessageReceive(object sender, EventArgs<DirectMessage> args)
        {
            this.DirectMessages.Insert(0, args.Value.BuildUp(this.Container));
        }
    }
}
