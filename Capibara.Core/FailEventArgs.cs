using System;
namespace Capibara
{
    public class FailEventArgs : EventArgs
    {
        public Exception Error { get; }

        public FailEventArgs(Exception error)
        {
            this.Error = error;
        }
        
        public static implicit operator FailEventArgs(Exception error)
        {
            return new FailEventArgs(error);
        }
    }
}
