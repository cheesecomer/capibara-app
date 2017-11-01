using System;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.Users;

using Microsoft.Practices.Unity;

namespace Capibara.Models
{
    public class User : ModelBase<User>
    {
        private int id;

        private string nickname;

        public event EventHandler SignUpSuccess;

        public event EventHandler<Exception> SignUpFail;

        public event EventHandler RefreshSuccess;

        public event EventHandler<Exception> RefreshFail;

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

        public override void Restore(User model)
        {
            base.Restore(model);

            this.Nickname = model.Nickname;
        }

        /// <summary>
        /// メールアドレスとパスワードでログインを行います
        /// </summary>
        /// <returns>The login.</returns>
        public async Task Refresh()
        {
            var request = new ShowRequest(this).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                if (this.IsolatedStorage.UserId == this.Id)
                {
                    this.IsolatedStorage.UserNickname = this.Nickname;
                    this.IsolatedStorage.Save();

                    this.Container.RegisterInstance(typeof(User), UnityInstanceNames.MyProfile, this);
                }

                this.SignUpSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.SignUpFail?.Invoke(this, e);
            }
        }

        /// <summary>
        /// ユーザ登録を行います
        /// </summary>
        /// <returns>The login.</returns>
        public async Task SignUp()
        {
            var request = new CreateRequest { Nickname = this.Nickname }.BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.IsolatedStorage.AccessToken = response.AccessToken;
                this.IsolatedStorage.UserId = response.UserId;
                this.IsolatedStorage.UserNickname = this.Nickname;
                this.IsolatedStorage.Save();

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.MyProfile, this);

                this.SignUpSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.SignUpFail?.Invoke(this, e);
            }
        }
    }
}
