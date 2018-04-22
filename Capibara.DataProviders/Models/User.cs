using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Unity;

namespace Capibara.Models
{
    public class User : ModelBase<User>
    {
        private int id;

        private string nickname;

        private string biography;

        private string iconUrl;

        private string iconThumbnailUrl;

        private string iconBase64;

        private bool isBlock;

        private bool isAccepted;

        private int? followId;

        public virtual event EventHandler SignUpSuccess;

        public virtual event EventHandler<FailEventArgs> SignUpFail;

        public virtual event EventHandler RefreshSuccess;

        public virtual event EventHandler<FailEventArgs> RefreshFail;

        public virtual event EventHandler CommitSuccess;

        public virtual event EventHandler<FailEventArgs> CommitFail;

        public virtual event EventHandler AcceptSuccess;

        public virtual event EventHandler<FailEventArgs> AcceptFail;

        public virtual event EventHandler BlockSuccess;

        public virtual event EventHandler<FailEventArgs> BlockFail;

        public virtual event EventHandler DestroySuccess;

        public virtual event EventHandler<FailEventArgs> DestroyFail;

        public virtual event EventHandler ReportSuccess;

        public virtual event EventHandler<FailEventArgs> ReportFail;

        public virtual event EventHandler ToggleFollowSuccess;

        public virtual event EventHandler<FailEventArgs> ToggleFollowFail;

        public virtual int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public virtual string Nickname
        {
            get => this.nickname;
            set => this.SetProperty(ref this.nickname, value);
        }

        public virtual string Biography
        {
            get => this.biography;
            set => this.SetProperty(ref this.biography, value);
        }

        [JsonProperty("icon_url")]
        public virtual string IconUrl
        {
            get => this.iconUrl;
            set => this.SetProperty(ref this.iconUrl, value);
        }

        [JsonProperty("icon_thumb_url")]
        public virtual string IconThumbnailUrl
        {
            get => this.iconThumbnailUrl;
            set => this.SetProperty(ref this.iconThumbnailUrl, value);
        }

        public virtual string IconBase64
        {
            get => this.iconBase64;
            set => this.SetProperty(ref this.iconBase64, value);
        }

        [JsonProperty("is_block")]
        public virtual bool IsBlock
        {
            get => this.isBlock;
            set => this.SetProperty(ref this.isBlock, value);
        }

        [JsonProperty("accepted")]
        public  virtual bool IsAccepted
        {
            get => this.isAccepted;
            set => this.SetProperty(ref this.isAccepted, value);
        }

        [JsonProperty("follow")]
        public virtual int? FollowId
        {
            get => this.followId;
            set => this.SetProperty(ref this.followId, value);
        }

        public virtual bool IsFollow => this.FollowId.HasValue;

        public virtual bool IsOwn => this.IsolatedStorage.UserId == this.Id;

        public override void Restore(User model)
        {
            base.Restore(model);

            this.Id = model.Id;
            this.Nickname = model.Nickname;
            this.Biography = model.Biography;
            this.IconUrl = model.IconUrl;
            this.IconThumbnailUrl = model.IconThumbnailUrl;
            this.IsBlock = model.IsBlock;
            this.IsAccepted = model.IsAccepted;
        }

        /// <summary>
        /// メールアドレスとパスワードでログインを行います
        /// </summary>
        /// <returns>The login.</returns>
        public virtual async Task<bool> Refresh()
        {
            var request = this.RequestFactory.UsersShowRequest(this).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.Restore(response);

                if (this.IsolatedStorage.UserId == this.Id)
                {
                    this.IsolatedStorage.UserNickname = this.Nickname;
                    this.IsolatedStorage.Save();

                    this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this);
                }

                this.RefreshSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.RefreshFail?.Invoke(this, e);
                return false;
            }
        }

        /// <summary>
        /// ユーザ登録を行います
        /// </summary>
        /// <returns>The login.</returns>
        public virtual async Task<bool> SignUp()
        {
            var request = this.RequestFactory.UsersCreateRequest(Nickname = this.Nickname).BuildUp(this.Container);
            try
            {
                var response = await request.Execute();

                this.Restore(response as User);

                this.IsolatedStorage.AccessToken = response.AccessToken;
                this.IsolatedStorage.UserId = response.Id;
                this.IsolatedStorage.UserNickname = this.Nickname;
                this.IsolatedStorage.Save();

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this);

                this.SignUpSuccess?.Invoke(this, null);
                return true;
            }
            catch (Exception e)
            {
                this.SignUpFail?.Invoke(this, e);
                return false;
            }
        }

        /// <summary>
        /// ユーザー情報を更新します。
        /// </summary>
        /// <returns>The commit.</returns>
        public virtual async Task<bool> Commit()
        {
            var request = this.RequestFactory.UsersUpdateRequest(this).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.Restore(response);

                this.IsolatedStorage.UserNickname = this.Nickname;
                this.IsolatedStorage.Save();

                if (this.Container.IsRegistered(typeof(User), UnityInstanceNames.CurrentUser))
                {
                    this.Container.Resolve<User>(UnityInstanceNames.CurrentUser).Restore(this);
                }
                else
                {
                    this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this);
                }

                this.CommitSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.CommitFail?.Invoke(this, e);
                return false;
            }
        }

        public virtual async Task<bool> Accept()
        {
            var request = this.RequestFactory.UsersUpdateRequest(true).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.Restore(response);

                this.IsolatedStorage.UserNickname = this.Nickname;
                this.IsolatedStorage.Save();

                if (this.Container.IsRegistered(typeof(User), UnityInstanceNames.CurrentUser))
                {
                    this.Container.Resolve<User>(UnityInstanceNames.CurrentUser).Restore(this);
                }
                else
                {
                    this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this);
                }

                this.AcceptSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.AcceptFail?.Invoke(this, e);
                return false;
            }
        }

        /// <summary>
        /// ユーザーをブロックします。
        /// </summary>
        /// <returns>The commit.</returns>
        public virtual async Task<bool> Block()
        {
            var request = this.RequestFactory.BlocksCreateRequest(this).BuildUp(this.Container);

            try
            {
                var response = await request.Execute();

                this.IsBlock = true;

                this.BlockSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.BlockFail?.Invoke(this, e);
                return false;
            }
        }

        /// <summary>
        /// ユーザー情報を削除します。
        /// </summary>
        /// <returns>The commit.</returns>
        public virtual async Task<bool> Destroy()
        {
            var request = this.RequestFactory.UsersDestroyRequest().BuildUp(this.Container);

            try
            {
                await request.Execute();

                this.IsolatedStorage.AccessToken = null;
                this.IsolatedStorage.UserId = 0;
                this.IsolatedStorage.UserNickname = null;
                this.IsolatedStorage.Save();

                this.DestroySuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.DestroyFail?.Invoke(this, e);
                return false;
            }
        }

        /// <summary>
        /// Report the specified reason and message.
        /// </summary>
        /// <returns>The report.</returns>
        /// <param name="reason">Reason.</param>
        /// <param name="message">Message.</param>
        public virtual async Task<bool> Report(ReportReason reason, string message)
        {
            var request = this.RequestFactory.ReportsCreateRequest(this, reason, message).BuildUp(this.Container);
            try
            {
                await request.Execute();

                this.ReportSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.ReportFail?.Invoke(this, e);

                return false;
            }
        }

        /// <summary>
        /// フォロー状態を切り替えます
        /// </summary>
        /// <returns>成否</returns>
        public virtual Task<bool> ToggleFollow()
        {
            Func<Task<bool>> func = null;
            if (this.IsFollow)
                func = this.Unfollow;
            else
                func = this.Follow;

            return func.Invoke();
        }

        private async Task<bool> Follow()
        {
            var request = this.RequestFactory.FollowsCreateRequest(this).BuildUp(this.Container);
            try
            {
                var response = await request.Execute();

                this.FollowId = response.Follow.Id;

                this.ToggleFollowSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.ToggleFollowFail?.Invoke(this, e);
                return false;
            }
        }

        private async Task<bool> Unfollow()
        {
            var request = this.RequestFactory.FollowsDestroyRequest(this.FollowId.Value).BuildUp(this.Container);
            try
            {
                await request.Execute();

                this.FollowId = null;

                this.ToggleFollowSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.ToggleFollowFail?.Invoke(this, e);
                return false;
            }
        }
    }
}
