using System;

using System.Threading.Tasks;

using Capibara.Models;

using Newtonsoft.Json;

using Unity;
using Unity.Attributes;

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

        [JsonProperty("image")]
        public string Image { get; set; }
    }

    public abstract class ChatChannelBase : ChannelBase<Message>
    {
        public abstract Task Speak(string message, string image);
    }

    /// <summary>
    /// チャットチャンネル
    /// </summary>
    public class ChatChannel : ChatChannelBase
    {
        private Room room;

        private IChannelIdentifier channelIdentifier;

        public override IChannelIdentifier ChannelIdentifier
            => this.channelIdentifier ?? (this.channelIdentifier = new ChatChannelIdentifier(this.room.Id));

        public ChatChannel(Room room)
        {
            this.room = room;
        }

        public override Task Speak(string message, string image)
        {
            var command = new MessageCommand<SpeakActionContext>(this.ChannelIdentifier, new SpeakActionContext { Message = message, Image = image });
            return this.Cable.SendCommand(command);
        }
    }
}
