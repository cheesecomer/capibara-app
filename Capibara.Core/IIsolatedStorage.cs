using System;
namespace Capibara
{
    public interface IIsolatedStorage
    {
        int UserId { get; set; }

        string UserNickname { get; set; }

        string AccessToken { get; set; }

        string OAuthRequestToken { get; set; }

        string OAuthRequestTokenSecret { get; set; }

        Uri OAuthCallbackUrl { get; set; }

        void Save();
    }
}
