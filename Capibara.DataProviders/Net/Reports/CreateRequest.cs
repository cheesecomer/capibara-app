using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Reports
{
    public class CreateRequest : RequestBase
    {
        private User target;

        private ReportReason reason;

        private string message;

        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "reports" };

        public override bool NeedAuthentication { get; } = true;

        [JsonProperty("target_id")]
        public int TargetId => this.target.Id;

        [JsonProperty("reason")]
        public ReportReason Reason => this.reason;

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message => this.message;

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public CreateRequest(User target, ReportReason reason, string message = null)
        {
            this.target = target;
            this.reason = reason;
            this.message = message;
        }
    }
}
