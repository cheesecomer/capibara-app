using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using NUnit.Framework;
using Moq;

namespace Capibara.Test.Net
{
    public class HttpContentHandler : HttpContent
    {
        public string ResultOfString { get; set; }

        public Stream ResultOfStream { get; set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            if (this.ResultOfString != null)
            {
                Task.Run(() => {
                    using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(this.ResultOfString)))
                        memStream.CopyTo(stream);
                    taskCompletionSource.SetResult(null);
                });
            }
            else if (this.ResultOfStream != null)
            {
                Task.Run(() => {
                    this.ResultOfStream.CopyTo(stream);
                    taskCompletionSource.SetResult(null);
                });
            }
            else
            {
                taskCompletionSource.SetResult(null);
            }

            return taskCompletionSource.Task;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = this.ResultOfString?.Length ?? ResultOfStream?.Length ?? 0;
            return true;
        }
    }
}
