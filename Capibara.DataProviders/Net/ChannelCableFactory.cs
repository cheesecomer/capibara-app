using System;

using Capibara.Net.Channels;

namespace Capibara.Net
{
    public interface IChannelCableFactory
    {
        ChannelCableBase Create();
    }

    public class ChannelCableFactory : IChannelCableFactory
    {
        ChannelCableBase IChannelCableFactory.Create() => new ChannelCable();
    }
}
