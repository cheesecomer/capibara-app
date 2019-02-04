using System;
namespace Capibara.Domain.Models
{
    public class User : ModelBase<User>
    {
        private int id;

        private string nickname;

        private string biography;

        private string iconUrl;

        private string iconThumbnailUrl;

        private string iconBase64;

        private int? blockId;

        private bool isAccepted;

        private int? followId;

        private bool isFollower;

        private int friendsCount;

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

        public virtual string IconUrl
        {
            get => this.iconUrl;
            set => this.SetProperty(ref this.iconUrl, value);
        }

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

        public virtual int? BlockId
        {
            get => this.blockId;
            set
            {
                this.SetProperty(ref this.blockId, value);
                this.RaisePropertyChanged(nameof(IsBlock));
            }
        }

        public virtual bool IsAccepted
        {
            get => this.isAccepted;
            set => this.SetProperty(ref this.isAccepted, value);
        }

        public virtual int? FollowId
        {
            get => this.followId;
            set
            {
                this.SetProperty(ref this.followId, value);
                this.RaisePropertyChanged(nameof(IsFollow));
            }
        }

        public virtual bool IsFollower
        {
            get => this.isFollower;
            set => this.SetProperty(ref this.isFollower, value);
        }

        public virtual int FriendsCount
        {
            get => this.friendsCount;
            set => this.SetProperty(ref this.friendsCount, value);
        }

        public virtual bool IsBlock => this.BlockId.HasValue;

        public virtual bool IsFollow => this.FollowId.HasValue;

        public override User Restore(User other)
        {
            this.Id = other.Id;
            this.Nickname = other.Nickname;
            this.Biography = other.Biography;
            this.IconUrl = other.IconUrl;
            this.IconThumbnailUrl = other.IconThumbnailUrl;
            this.BlockId = other.BlockId;
            this.IsAccepted = other.IsAccepted;
            this.FollowId = other.FollowId;
            this.IsFollower = other.IsFollower;
            this.FriendsCount = other.FriendsCount;

            return this;
        }
    }
}
