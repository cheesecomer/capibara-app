using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Auth;

using Foundation;

namespace Capibara.iOS
{
    public class SecureIsolatedStorage : ISecureIsolatedStorage
    {
        public SecureIsolatedStorage()
        {
#if USE_USER_DEFAULTS
            var pref = NSUserDefaults.StandardUserDefaults;
            this.UserNickname = pref.StringForKey("user_nickname");
            this.UserId = (int)pref.IntForKey("AUTH.user_id");
            this.AccessToken = pref.StringForKey("AUTH.access_token");
#else
            var properties = AccountStore.Create().FindAccountsForService("com.cheese-comer.Capibara").SingleOrDefault()?.Properties;

            this.Email = properties?.ValueOrDefault("email");
            this.AccessToken = properties?.ValueOrDefault("access_token");
#endif
        }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string AccessToken { get; set; }

        public void Save()
        {
#if USE_USER_DEFAULTS
            var pref = NSUserDefaults.StandardUserDefaults;
            pref.SetString(this.UserNickname ?? string.Empty, "user_nickname");
            pref.SetInt(this.UserId, "AUTH.user_id");
            pref.SetString(this.AccessToken ?? string.Empty, "AUTH.access_token");
            pref.Synchronize();
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
