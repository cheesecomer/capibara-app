using System;

using Foundation;

namespace Capibara.iOS
{
    public class IsolatedStorage : IIsolatedStorage
    {
        public IsolatedStorage()
        {

            var preference = NSUserDefaults.StandardUserDefaults;
            this.UserNickname = preference.StringForKey("user_nickname");

#if USE_USER_DEFAULTS
            this.UserId = (int)preference.IntForKey("AUTH.user_id");
            this.AccessToken = preference.StringForKey("AUTH.access_token");
#else
            var properties = AccountStore.Create().FindAccountsForService("com.cheesecomer.Capibara").SingleOrDefault()?.Properties;

            this.Email = properties?.ValueOrDefault("email");
            this.AccessToken = properties?.ValueOrDefault("access_token");
#endif
        }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string AccessToken { get; set; }

        public void Save()
        {
            var preference = NSUserDefaults.StandardUserDefaults;
            preference.SetString(this.UserNickname ?? string.Empty, "user_nickname");

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
            AccountStore.Create().Save(account, "com.cheesecomer.Capibara");
#endif
        }
    }
}
