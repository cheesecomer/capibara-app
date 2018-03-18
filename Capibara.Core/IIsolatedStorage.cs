using System;
namespace Capibara
{
    public interface IIsolatedStorage
    {
        int UserId { get; set; }

        string UserNickname { get; set; }

        string AccessToken { get; set; }

        Net.OAuth.TokenPair OAuthRequestTokenPair { get; set; }

        Uri OAuthCallbackUrl { get; set; }

        void Save();
    }
}
