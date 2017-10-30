using System;
using System.Net.WebSockets;

namespace Capibara.Net
{
    public static class WebSocketMessageTypeExtensions
    {
        public static bool IsText(this WebSocketMessageType source)
            => source == WebSocketMessageType.Text;

        public static bool IsClose(this WebSocketMessageType source)
            => source == WebSocketMessageType.Close;

        public static bool IsBinary(this WebSocketMessageType source)
            => source == WebSocketMessageType.Binary;
    }
}
