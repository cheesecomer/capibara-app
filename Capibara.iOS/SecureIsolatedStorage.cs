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
            this.UserId = (int)pref.IntForKey("AUTH.user_id");
            this.Email = pref.StringForKey("AUTH.email");
            this.AccessToken = pref.StringForKey("AUTH.access_token");
#else
            var properties = AccountStore.Create().FindAccountsForService("com.cheese-comer.Capibara").SingleOrDefault()?.Properties;

            this.Email = properties?.ValueOrDefault("email");
            this.AccessToken = properties?.ValueOrDefault("access_token");
#endif
        }

        public int UserId { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public void Save()
        {
#if USE_USER_DEFAULTS
            var pref = NSUserDefaults.StandardUserDefaults;
            pref.SetInt(this.UserId, "AUTH.user_id");
            pref.SetString(this.Email ?? string.Empty, "AUTH.email");
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
