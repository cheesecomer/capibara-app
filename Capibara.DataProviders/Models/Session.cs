using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.Sessions;

using Microsoft.Practices.Unity;

namespace Capibara.Models
{
    public class Session : ModelBase<Session>
    {
        private string email;

        private string password;

        public event EventHandler SignInSuccess;

        public event EventHandler<Exception> SignInFail;

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
        public async Task SignIn()
        {
            var request = new CreateRequest()
                {
                    Email = this.Email,
                    Password = this.Password
                }.BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.IsolatedStorage.AccessToken = response.AccessToken;
                this.IsolatedStorage.UserNickname = response.Nickname;
                this.IsolatedStorage.UserId = response.UserId;
                this.IsolatedStorage.Save();

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, new User { Id = response.UserId, Nickname = response.Nickname, Biography = response.Biography });

                this.SignInSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.SignInFail?.Invoke(this, e);
            }
        }
    }
}
