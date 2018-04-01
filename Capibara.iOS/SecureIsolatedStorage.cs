using System;

using Foundation;

namespace Capibara.iOS
{
    public class IsolatedStorage : IIsolatedStorage
    {
        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string AccessToken { get; set; }

        public IsolatedStorage()
        {

            var preference = NSUserDefaults.StandardUserDefaults;
            this.UserNickname = preference.StringForKey("user_nickname");
            this.UserId = (int)preference.IntForKey("AUTH.user_id");
            this.AccessToken = preference.StringForKey("AUTH.access_token");
        }

        public void Save()
        {
            var preference = NSUserDefaults.StandardUserDefaults;
            preference.SetString(this.UserNickname ?? string.Empty, "user_nickname");
            preference.SetInt(this.UserId, "AUTH.user_id");
            preference.SetString(this.AccessToken ?? string.Empty, "AUTH.access_token");
        }
    }
}
