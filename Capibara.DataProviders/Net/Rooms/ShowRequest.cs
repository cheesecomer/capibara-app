using System.Net.Http;

using Capibara.Models;

namespace Capibara.Net.Rooms
{
    public class ShowRequest : RequestBase<Room>
    {
        private Room room;

        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths
            => new string[] { "rooms", $"{this.room.Id}" };

        public ShowRequest(Room room)
        {
            this.room = room;
        }
    }
}
