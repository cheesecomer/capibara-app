using System;
namespace Capibara
{
    public interface IIsolatedStorage
    {
        event EventHandler Saved;

        int UserId { get; set; }

        string UserNickname { get; set; }

        string AccessToken { get; set; }

        void Save();
    }
}
