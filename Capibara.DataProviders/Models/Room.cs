using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Net.Channels;

using Newtonsoft.Json;

namespace Capibara.Models
{
    /// <summary>
    /// チャットルームモデル
    /// </summary>
    public class Room : ModelBase<Room>
    {
        private bool isConnected;

        private int id;

        private string name;

        private int capacity;

        private int numberOfParticipants;

        private ChatChannelBase channel;

        public virtual event EventHandler RefreshSuccess;

        public virtual event EventHandler<FailEventArgs> RefreshFail;

        public virtual event EventHandler SpeakSuccess;

        public virtual event EventHandler<FailEventArgs> SpeakFail;

        public virtual event EventHandler Disconnected;

        public virtual event EventHandler<User> JoinUser;

        public virtual event EventHandler<User> LeaveUser;

        public virtual event EventHandler RejectSubscription;

        public virtual bool IsConnected
        {
            get => this.isConnected;
            set => this.SetProperty(ref this.isConnected, value);
        }

        public ObservableCollection<Message> Messages { get; }
            = new ObservableCollection<Message>();

        public ObservableCollection<User> Participants { get; }
            = new ObservableCollection<User>();

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        public int Capacity
        {
            get => this.capacity;
            set => this.SetProperty(ref this.capacity, value);
        }

        [JsonProperty("number_of_participants")]
        public int NumberOfParticipants
        {
            get => this.numberOfParticipants;
            set => this.SetProperty(ref this.numberOfParticipants, value);
        }

        public virtual async Task<bool> Refresh()
        {
            var request = this.RequestFactory.RoomsShowRequest(this).BuildUp(this.Container);
            try
            {
                var result = await request.Execute();

                this.Restore(result);

                result.Messages?.Where(x => this.Messages.All(y => y.Id != x.Id))?.ForEach(x => this.Messages.Insert(0, x.BuildUp(this.Container)));

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
            if (this.channel != null)
            {
                return true;
            }

            this.channel = this.ChannelFactory.CreateChantChannel(this).BuildUp(this.Container);

            this.channel.Connected += (sender, e) => this.IsConnected = true;
            this.channel.MessageReceive += this.OnMessageReceive;
            this.channel.Disconnected += async (sender, e) =>
            {
                await this.Close(); 
                this.Disconnected?.Invoke(this, null);
            };

            this.channel.RejectSubscription += (sender, e) => this.RejectSubscription?.Invoke(this, null);

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

        public override void Restore(Room model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Capacity = model.Capacity;
            this.NumberOfParticipants = model.NumberOfParticipants;

            // 差分を追加
            model.Participants
                ?.Where(x => this.Participants.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Participants.Add(x.BuildUp(this.Container)));

            // 差分を削除
            this.Participants
                ?.Where(x => model.Participants.All(v => v.Id != x.Id))
                ?.ToList()
                ?.ForEach(x => this.Participants.Remove(x));
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

        private void OnMessageReceive(object sender, EventArgs<Message> args)
        {
            if (args.Value.Id != 0)
            {
                this.Messages.Insert(0, args.Value.BuildUp(this.Container));
            }
            else
            {
                this.OnSystemMessageReceive(args.Value);
            }
        }

        private void OnSystemMessageReceive(Message message)
        {
            if (message.Content.IsNullOrEmpty())
                return;

            var content = JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Content);
            var type = content.ValueOrDefault("type") as string;
            if (type.IsNullOrEmpty())
                return;

            if (type == "join_user" || type == "leave_user")
            {
                this.NumberOfParticipants
                    = content.ValueOrDefault("number_of_participants").ToInt();

                var user
                    = JsonConvert.DeserializeObject<User>(content.ValueOrDefault("user").ToString());

                if (type == "join_user")
                {
                    this.JoinUser?.Invoke(this, user);
                    if (!this.Participants.Any(x => x.Id == user.Id))
                    {
                        this.Participants.Add(user);
                    }
                }
                else
                {
                    this.LeaveUser?.Invoke(this, user);
                    if (this.Participants.Any(x => x.Id == user.Id))
                    {
                        this.Participants.Remove(this.Participants.FirstOrDefault(x => x.Id == user.Id));
                    }
                }
            }
        }
    }
}
