using System;
namespace Capibara.Models
{
    public class Block : ModelBase<Block>
    {
        private int id;

        private User target;

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public User Target
        {
            get => this.target;
            set => this.SetProperty(ref this.target, value);
        }
    }
}
