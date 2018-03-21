using System;
using System.Threading.Tasks;

using Unity;

namespace Capibara.Models
{
    public class Session : ModelBase<Session>
    {
        private string email;

        private string password;

        public virtual event EventHandler SignInSuccess;

        public virtual event EventHandler<FailEventArgs> SignInFail;

        /// <summary>
        /// メールアドレスを取得または設定します
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        /// <summary>
        /// パスワードを取得または設定します
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get => this.password;
            set => this.SetProperty(ref this.password, value);
        }

        /// <summary>
        /// メールアドレスとパスワードでログインを行います
        /// </summary>
        /// <returns>The login.</returns>
        public virtual async Task<bool> SignIn()
        {
            var request = this.RequestFactory.SessionsCreateRequest(this.Email, this.Password).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.IsolatedStorage.AccessToken = response.AccessToken;
                this.IsolatedStorage.UserNickname = response.Nickname;
                this.IsolatedStorage.UserId = response.Id;
                this.IsolatedStorage.Save();

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, response);

                this.SignInSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.SignInFail?.Invoke(this, e);

                return false;
            }
        }
    }
}
