using System;
using System.Collections.Generic;

namespace Capibara.Models
{
    public class FollowComparer : IEqualityComparer<Follow>
    {
        public bool Equals(Follow x, Follow y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return  x.Id == y.Id;
        }

        public int GetHashCode(Follow room)
        {
            return room.GetHashCode();
        }
    }
}
