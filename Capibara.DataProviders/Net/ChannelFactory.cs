﻿using System;

using Capibara.Net.Channels;
using Capibara.Models;

namespace Capibara.Net
{
    public interface IChannelFactory
    {
        ChatChannelBase CreateChantChannel(Room room);
    }

    public class ChannelFactory : IChannelFactory
    {
        ChatChannelBase IChannelFactory.CreateChantChannel(Room room) => new ChatChannel(room);
    }
}
