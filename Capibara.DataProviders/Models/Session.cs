using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.Sessions;

namespace Capibara.Models
{
    public class Session : ModelBase<Session>
    {
        private string email;

        private string password;

        private Error error;

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
        /// APIエラー
        /// </summary>
        /// <value>The error.</value>
        public Error Error
        {
            get => this.error;
            set => this.SetProperty(ref this.error, value);
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

                this.Error = null;

                this.SecureIsolatedStorage.Email = this.Email;
                this.SecureIsolatedStorage.AccessToken = response.AccessToken;
                this.SecureIsolatedStorage.UserId = response.UserId;
                this.SecureIsolatedStorage.Save();

                this.SignInSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                if (e is HttpUnauthorizedException)
                {
                    this.Error = (e as HttpUnauthorizedException).Detail;
                }

                this.SignInFail?.Invoke(this, e);
            }
        }
    }
}
