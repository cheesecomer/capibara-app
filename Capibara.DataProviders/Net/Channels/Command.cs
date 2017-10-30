using Newtonsoft.Json;

namespace Capibara.Net.Channels
{

    public interface ICommand
    {
        string Command { get; }
    }

    public interface IChannelIdentifier
    {
        string Channel { get; }
    }

    public class SubscribeCommand : ICommand
    {
        public SubscribeCommand(IChannelIdentifier identifier)
        {
            this.Identifier = identifier;
        }

        [JsonProperty("command")]
        public string Command { get; } = "subscribe";

        [JsonIgnore]
        public IChannelIdentifier Identifier { get; }

        [JsonProperty("identifier")]
        public string SerializedIdentifier => JsonConvert.SerializeObject(this.Identifier);
    }

    public interface IActionContext
    {
        string Action { get; }
    }

    public class MessageCommand<TContext> : ICommand where TContext : IActionContext
    {
        public MessageCommand(IChannelIdentifier identifier, TContext context)
        {
            this.Identifier = identifier;
            this.Context = context;
        }

        [JsonProperty("command")]
        public string Command { get; } = "message";

        [JsonIgnore]
        public IChannelIdentifier Identifier { get; }

        [JsonProperty("identifier")]
        public string SerializedIdentifier => JsonConvert.SerializeObject(this.Identifier);

        [JsonIgnore]
        public TContext Context { get; }

        [JsonProperty("data")]
        public string SerializedContext => JsonConvert.SerializeObject(this.Context);
    }
}
