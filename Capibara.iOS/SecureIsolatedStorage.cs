using System;
using Capibara.Net.OAuth;

using Foundation;

namespace Capibara.iOS
{
    public class IsolatedStorage : IIsolatedStorage
    {
        public IsolatedStorage()
        {

            var preference = NSUserDefaults.StandardUserDefaults;
            var oAuthRequestToken = preference.StringForKey("OAUTH.request_token");
            var oAuthRequestTokenSecret = preference.StringForKey("OAUTH.request_token_secret");
            var oauthCallbackUrl = preference.StringForKey("OAUTH.callback_url");

            this.UserNickname = preference.StringForKey("user_nickname");
            this.OAuthRequestTokenPair = new TokenPair
            {
                Token = oAuthRequestToken,
                TokenSecret = oAuthRequestTokenSecret
            };
            this.OAuthCallbackUrl = oauthCallbackUrl.IsPresent() ? new Uri(oauthCallbackUrl) : null;

#if USE_USER_DEFAULTS
            this.UserId = (int)preference.IntForKey("AUTH.user_id");
            this.AccessToken = preference.StringForKey("AUTH.access_token");
#else
            var properties = AccountStore.Create().FindAccountsForService("com.cheese-comer.Capibara").SingleOrDefault()?.Properties;

            this.Email = properties?.ValueOrDefault("email");
            this.AccessToken = properties?.ValueOrDefault("access_token");
#endif
        }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string AccessToken { get; set; }

        public TokenPair OAuthRequestTokenPair { get; set; }

        public Uri OAuthCallbackUrl { get; set; }

        public void Save()
        {
            var preference = NSUserDefaults.StandardUserDefaults;
            preference.SetString(this.UserNickname ?? string.Empty, "user_nickname");
            preference.SetString(this.OAuthRequestTokenPair?.Token ?? string.Empty, "OAUTH.request_token");
            preference.SetString(this.OAuthRequestTokenPair?.TokenSecret ?? string.Empty, "OAUTH.request_token_secret");
            preference.SetString(this.OAuthCallbackUrl?.AbsoluteUri ?? string.Empty, "OAUTH.callback_url");

#if USE_USER_DEFAULTS
            preference.SetInt(this.UserId, "AUTH.user_id");
            preference.SetString(this.AccessToken ?? string.Empty, "AUTH.access_token");
#else
            var properties = new Dictionary<string, string>()
            {
                { "email", this.Email },
                { "access_token", this.AccessToken }
            };

            var account = new Account(this.Email, properties);
            AccountStore.Create().Save(account, "com.cheese-comer.Capibara");
#endif
        }
    }
}
