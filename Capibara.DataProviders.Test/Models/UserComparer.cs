using System.Collections.Generic;

using Capibara.Models;

namespace Capibara.Test.Models
{
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return
                    x.Nickname == y.Nickname
                &&  x.Id == y.Id;
        }

        public int GetHashCode(User room)
        {
            return room.GetHashCode();
        }
    }
}
