using System;
namespace Capibara
{
    public interface IIsolatedStorage
    {
        int UserId { get; set; }

        string UserNickname { get; set; }

        string AccessToken { get; set; }

        void Save();
    }
}
