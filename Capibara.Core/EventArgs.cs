using System;
namespace Capibara
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; }
        
        public EventArgs(T value)
        {
            this.Value = value;
        }
        
        public static implicit operator EventArgs<T>(T value)
        {
            return new EventArgs<T>(value);
        }

        public static implicit operator T(EventArgs<T> args)
        {
            return args.Value;
        }
    }
}
