using System;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.Users;

namespace Capibara.Models
{
    public class User : ModelBase<User>
    {
        private int id;

        private string nickname;

        private Error error;

        public event EventHandler SignUpSuccess;

        public event EventHandler<Exception> SignUpFail;

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string Nickname
        {
            get => this.nickname;
            set => this.SetProperty(ref this.nickname, value);
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
        public async Task SignUp()
        {
            var request = new CreateRequest { Nickname = this.Nickname }.BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.Error = null;

                this.SecureIsolatedStorage.AccessToken = response.AccessToken;
                this.SecureIsolatedStorage.UserId = response.UserId;
                this.SecureIsolatedStorage.Save();

                this.SignUpSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                if (e is HttpUnauthorizedException)
                {
                    this.Error = (e as HttpUnauthorizedException).Detail;
                }

                this.SignUpFail?.Invoke(this, e);
            }
        }
    }
}
