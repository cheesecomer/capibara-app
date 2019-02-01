using System;
using System.Collections.Generic;

namespace Capibara.Models
{
    public class RoomComparer : IEqualityComparer<Room>
    {
        public bool Equals(Room x, Room y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return
                    x.Name == y.Name
                &&  x.Capacity == y.Capacity
                &&  x.Id == y.Id
                &&  x.NumberOfParticipants == y.NumberOfParticipants;
        }

        public int GetHashCode(Room room)
        {
            return room.GetHashCode();
        }
    }
}
