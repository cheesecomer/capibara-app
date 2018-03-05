using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Net.Channels;
using Capibara.Net.Rooms;

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

        private ChatChannel channel;

        public event EventHandler RefreshSuccess;

        public event EventHandler<Exception> RefreshFail;

        public event EventHandler SpeakSuccess;

        public event EventHandler<Exception> SpeakFail;

        public event EventHandler Disconnected;

        public event EventHandler<User> JoinUser;

        public event EventHandler<User> LeaveUser;

        public bool IsConnected
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

        public async Task Refresh()
        {
            var request = new ShowRequest(this).BuildUp(this.Container);
            try
            {
                var result = await request.Execute();

                this.Restore(result);

                result.Messages?.Where(x => this.Messages.All(y => y.Id != x.Id))?.ForEach(x => this.Messages.Insert(0, x.BuildUp(this.Container)));

                this.RefreshSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.RefreshFail?.Invoke(this, e);
            }
        }

        public async Task<bool> Connect()
        {
            if (this.channel != null)
            {
                return true;
            }

            this.channel = new ChatChannel(this).BuildUp(this.Container);

            this.channel.Connected += (sender, e) => this.IsConnected = true;
            this.channel.MessageReceive += this.OnMessageReceive;
            this.channel.Disconnected += (sender, e) => this.Disconnected?.Invoke(this, null);
            this.channel.Disconnected += (sender, e) => this.IsConnected = false;

            return await this.channel.Connect();
        }

        public async Task Close()
        {
            this.IsConnected = false;
            if (this.channel != null)
            {
                await this.channel.Close();
                this.channel.Dispose();
            }

            this.channel = null;
        }

        public override void Restore(Room model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Capacity = model.Capacity;
            this.NumberOfParticipants = model.NumberOfParticipants;
            this.Participants.Clear();
            model.Participants?.ForEach(x => this.Participants.Add(x.BuildUp(this.Container)));
        }

        public async Task Speak(string message)
        {
            try
            {
                await this.channel.Speak(message);

                this.SpeakSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.SpeakFail?.Invoke(this, e);
            }
        }

        private void OnMessageReceive(object sender, Message message)
        {
            if (message.Id != 0)
            {
                this.Messages.Insert(0, message.BuildUp(this.Container));
            }
            else
            {
                this.OnSystemMessageReceive(message);
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
