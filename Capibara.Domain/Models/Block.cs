namespace Capibara.Domain.Models
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

        public override Block Restore(Block other)
        {
            this.Id = other.Id;
            this.Target = other.Target;

            return this;
        }
    }
}
