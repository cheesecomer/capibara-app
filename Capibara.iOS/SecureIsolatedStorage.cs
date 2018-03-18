using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Auth;

using Foundation;

namespace Capibara.iOS
{
    public class IsolatedStorage : IIsolatedStorage
    {
        public IsolatedStorage()
        {

            var preference = NSUserDefaults.StandardUserDefaults;
            var oauthCallbackUrl = preference.StringForKey("OAUTH.callback_url");
            this.UserNickname = preference.StringForKey("user_nickname");
            this.OAuthRequestToken = preference.StringForKey("OAUTH.request_token");
            this.OAuthRequestTokenSecret = preference.StringForKey("OAUTH.request_token_secret");
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

        public string OAuthRequestToken { get; set; }

        public string OAuthRequestTokenSecret { get; set; }

        public Uri OAuthCallbackUrl { get; set; }

        public void Save()
        {
            var preference = NSUserDefaults.StandardUserDefaults;
            preference.SetString(this.UserNickname ?? string.Empty, "user_nickname");
            preference.SetString(this.OAuthRequestToken ?? string.Empty, "OAUTH.request_token");
            preference.SetString(this.OAuthRequestTokenSecret ?? string.Empty, "OAUTH.request_token_secret");
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
