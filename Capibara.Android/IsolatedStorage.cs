using System;

using Android.Content;

using Capibara;

using Plugin.GoogleAnalytics;

namespace Capibara.Droid
{
    public class IsolatedStorage : IIsolatedStorage
    {
        private int userId;

        public event EventHandler Saved;

        public int UserId
        {
            get => this.userId;
            set
            {
                this.userId = value;

                GoogleAnalytics.Current.Tracker.UserId = value == 0 ? string.Empty : $"{value}";
            }
        }

        public string UserNickname { get; set; }

        public string AccessToken { get; set; }

        public IsolatedStorage()
        {
            var preferences = MainActivity.Instance.GetPreferences(FileCreationMode.Private);

            this.UserNickname = preferences.GetString("user_nickname", string.Empty);
            this.UserId = preferences.GetInt("AUTH.user_id", 0);
            this.AccessToken = preferences.GetString("AUTH.access_token", string.Empty);

            preferences.Dispose();
        }

        public void Save()
        {
            var preferences = MainActivity.Instance.GetPreferences(FileCreationMode.Private);

            preferences
                .Edit()
                .PutString("user_nickname", this.UserNickname)
                .PutString("AUTH.access_token", this.AccessToken)
                .PutInt("AUTH.user_id", this.UserId)
                .Commit();

            preferences.Dispose();
        }
    }
}
