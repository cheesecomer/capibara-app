using System;
namespace Capibara.Domain.Models
{
    public class Inquiry : ModelBase<Inquiry>
    {
        private string email;

        private string message;

        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        public override Inquiry Restore(Inquiry other)
        {
            this.Email = other.Email;
            this.Message = other.Message;

            return this;
        }
    }
}
