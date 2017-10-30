using System;
namespace Capibara
{
    public interface ISecureIsolatedStorage
    {
        int UserId { get; set; }

        string Email { get; set; }

        string AccessToken { get; set; }

        void Save();
    }
}
