using System;
namespace Capibara.Models
{
    public class Follow : ModelBase<Follow>
    {
        private int id;

        public virtual int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }
    }
}
