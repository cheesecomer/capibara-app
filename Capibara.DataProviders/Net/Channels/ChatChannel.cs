using System;

using System.Threading.Tasks;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Channels
{
    /// <summary>
    /// チャンネル識別子
    /// </summary>
    public class ChatChannelIdentifier : IChannelIdentifier
    {
        [JsonProperty("channel")]
        public string Channel { get; } = "ChatChannel";

        [JsonProperty("room_id")]
        public int RoomId { get; }

        public ChatChannelIdentifier(int roomId)
        {
            this.RoomId = roomId;
        }
    }

    public class SpeakActionContext : IActionContext
    {
        [JsonProperty("action")]
        public string Action { get; } = "speak";

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// チャットチャンネル
    /// </summary>
    public class ChatChannel : ChannelBase<Message>
    {
        private Room room;

        private IChannelIdentifier channelIdentifier;

        protected override IChannelIdentifier ChannelIdentifier
            => this.channelIdentifier ?? (this.channelIdentifier = new ChatChannelIdentifier(this.room.Id));

        public ChatChannel(Room room)
        {
            this.room = room;
        }

        public Task Speak(string message)
        {
            var command = new MessageCommand<SpeakActionContext>(this.ChannelIdentifier, new SpeakActionContext() { Message = message });
            return this.Cable.SendCommand(command);
        }
    }
}
