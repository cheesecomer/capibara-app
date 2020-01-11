using System;
namespace Capibara.Domain.Models
{
    public class Report : ModelBase<Report>
    {
        private User target;

        private ReportReason? reason;

        private string message;

        public User Target
        {
            get => this.target;
            set => this.SetProperty(ref this.target, value);
        }

        public ReportReason? Reason
        {
            get => this.reason;
            set => this.SetProperty(ref this.reason, value);
        }

        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        public Report()
        {
        }

        public override Report Restore(Report other)
        {
            this.Target = other.Target;
            this.Reason = other.Reason;
            this.Message = other.Message;

            return this;
        }
    }
}
