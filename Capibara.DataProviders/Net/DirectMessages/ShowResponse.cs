using System;
using System.Collections.Generic;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.DirectMessages
{
    public class ShowResponse
    {
        [JsonProperty("direct_messages")]
        public List<DirectMessage> DirectMessages { get; set; } = new List<DirectMessage>();
    }
}
