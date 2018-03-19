using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Net.OAuth;
using Capibara.Net.Users;

using Newtonsoft.Json;

using Unity;
using Unity.Attributes;

using BlockRequest = Capibara.Net.Blocks.CreateRequest;
using ReportRequest = Capibara.Net.Reports.CreateRequest;

namespace Capibara.Models
{
    public class User : ModelBase<User>
    {
        private int id;

        private string nickname;

        private string biography;

        private string iconUrl;

        private string iconBase64;

        private bool isBlock;

        public virtual event EventHandler SignUpSuccess;

        public virtual event EventHandler<Exception> SignUpFail;

        public virtual event EventHandler RefreshSuccess;

        public virtual event EventHandler<Exception> RefreshFail;

        public virtual event EventHandler CommitSuccess;

        public virtual event EventHandler<Exception> CommitFail;

        public virtual event EventHandler BlockSuccess;

        public virtual event EventHandler<Exception> BlockFail;

        public virtual event EventHandler<Uri> OAuthAuthorizeSuccess;

        public virtual event EventHandler<Exception> OAuthAuthorizeFail;

        public virtual event EventHandler DestroySuccess;

        public virtual event EventHandler<Exception> DestroyFail;

        public virtual event EventHandler ReportSuccess;

        public virtual event EventHandler<Exception> ReportFail;

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

        public string Biography
        {
            get => this.biography;
            set => this.SetProperty(ref this.biography, value);
        }

        [JsonProperty("icon_url")]
        public string IconUrl
        {
            get => this.iconUrl;
            set => this.SetProperty(ref this.iconUrl, value);
        }

        public string IconBase64
        {
            get => this.iconBase64;
            set => this.SetProperty(ref this.iconBase64, value);
        }

        [JsonProperty("is_block")]
        public bool IsBlock
        {
            get => this.isBlock;
            set => this.SetProperty(ref this.isBlock, value);
        }

        [Dependency]
        public ITwitterOAuthService TwitterOAuthService { get; set; }

        public bool IsOwn => this.IsolatedStorage.UserId == this.Id;

        public override void Restore(User model)
        {
            base.Restore(model);

            this.Id = model.Id;
            this.Nickname = model.Nickname;
            this.Biography = model.Biography;
            this.IconUrl = model.IconUrl;
            this.IsBlock = model.IsBlock;
        }

        /// <summary>
        /// メールアドレスとパスワードでログインを行います
        /// </summary>
        /// <returns>The login.</returns>
        public async Task<bool> Refresh()
        {
            var request = new ShowRequest(this).BuildUp(this.Container);

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
        public async Task<bool> SignUp()
        {
            var request = new CreateRequest { Nickname = this.Nickname }.BuildUp(this.Container);
            try
            {
                var response = await request.Execute();

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

        public virtual async Task<bool> OAuthAuthorize(OAuthProvider provider)
        {
            try
            {
                Uri authorizeUri = null;
                if (provider == OAuthProvider.Twitter)
                {
                    var session =
                        await this.TwitterOAuthService.AuthorizeAsync();

                    this.IsolatedStorage.OAuthRequestTokenPair = session.RequestToken;
                    this.IsolatedStorage.Save();

                    authorizeUri = session.AuthorizeUri;
                }
                else
                {
                    this.OAuthAuthorizeFail?.Invoke(this, new ArgumentException("Invalid OAuthProvider. Can use Twitter only"));
                    return false;
                }

                this.OAuthAuthorizeSuccess?.Invoke(this, authorizeUri);
                return true;
            }
            catch (Exception e)
            {
                this.OAuthAuthorizeFail?.Invoke(this, e);
                return false;
            }
        }

        public virtual async Task<bool> SignUpWithOAuth()
        {
            var path = this.IsolatedStorage.OAuthCallbackUrl.LocalPath;
            var provider = path.Split('/').Skip(1).ElementAtOrDefault(1);
            try
            {
                var query = this.IsolatedStorage.OAuthCallbackUrl.Query
                   .Replace("?", string.Empty).Split('&')
                   .Select(x => x.Split('='))
                   .Where(x => x.Length == 2)
                   .ToDictionary(x => x.First(), x => x.Last());

                var tokens = await this.TwitterOAuthService.GetAccessTokenAsync(
                    this.IsolatedStorage.OAuthRequestTokenPair,
                    query["oauth_verifier"]);

                var request = new Net.Sessions.CreateRequest()
                {
                    Provider = provider.ToLower(),
                    AccessToken = tokens.Token,
                    AccessTokenSecret = tokens.TokenSecret
                }.BuildUp(this.Container);

                var response = await request.Execute();

                this.IsolatedStorage.AccessToken = response.AccessToken;
                this.IsolatedStorage.UserNickname = response.Nickname;
                this.IsolatedStorage.UserId = response.Id;
                this.IsolatedStorage.OAuthCallbackUrl = null;
                this.IsolatedStorage.OAuthRequestTokenPair = null;
                this.IsolatedStorage.Save();

                this.Restore(response);

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, response);

                this.SignUpSuccess?.Invoke(this, null);

                return true;
            }
            catch (Exception e)
            {
                this.IsolatedStorage.OAuthCallbackUrl = null;
                this.IsolatedStorage.OAuthRequestTokenPair = null;
                this.IsolatedStorage.Save();

                this.SignUpFail?.Invoke(this, e);

                return false;
            }
        }

        /// <summary>
        /// ユーザー情報を更新します。
        /// </summary>
        /// <returns>The commit.</returns>
        public async Task<bool> Commit()
        {
            var request = new UpdateRequest(this).BuildUp(this.Container);

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

        /// <summary>
        /// ユーザーをブロックします。
        /// </summary>
        /// <returns>The commit.</returns>
        public async Task<bool> Block()
        {
            var request = new BlockRequest(this).BuildUp(this.Container);

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
            var request = new DestroyRequest().BuildUp(this.Container);

            try
            {
                await request.Execute();

                this.IsolatedStorage.AccessToken = null;
                this.IsolatedStorage.OAuthCallbackUrl = null;
                this.IsolatedStorage.OAuthRequestTokenPair = null;
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
            var request = new ReportRequest(this, reason, message).BuildUp(this.Container);
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
    }
}
