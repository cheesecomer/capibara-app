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
    public class DirectMessageChannelIdentifier : IChannelIdentifier
    {
        [JsonProperty("channel")]
        public string Channel { get; } = "DirectMessageChannel";

        [JsonProperty("addressee_id")]
        public int AddresseeId { get; }

        public DirectMessageChannelIdentifier(int addresseeId)
        {
            this.AddresseeId = addresseeId;
        }
    }

    public abstract class DirectMessageChannelBase : ChannelBase<DirectMessage>
    {
        public abstract Task Speak(string message);
    }

    public class DirectMessageChannel : DirectMessageChannelBase
    {
        private User addressee;

        private IChannelIdentifier channelIdentifier;

        public override IChannelIdentifier ChannelIdentifier
        => this.channelIdentifier ?? (this.channelIdentifier = new DirectMessageChannelIdentifier(this.addressee.Id));

        public DirectMessageChannel(User addressee)
        {
            this.addressee = addressee;
        }

        public override Task Speak(string message)
        {
            var command = new MessageCommand<SpeakActionContext>(this.ChannelIdentifier, new SpeakActionContext { Message = message });
            return this.Cable.SendCommand(command);
        }

        public class SpeakActionContext : IActionContext
        {
            [JsonProperty("action")]
            public string Action { get; } = "speak";

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
