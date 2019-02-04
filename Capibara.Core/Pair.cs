namespace Capibara
{
    public class Pair<TFirst, TSecond>
    {
        public TFirst First { get; }
        public TSecond Second { get; }

        public Pair(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        public override string ToString()
        {
            return $"({this.First} to {this.Second})";
        }

        public static bool operator !=(Pair<TFirst, TSecond> x, Pair<TFirst, TSecond> y)
        {
            return !x.Equals(y);
        }

        public static bool operator ==(Pair<TFirst, TSecond> x, Pair<TFirst, TSecond> y)
        {
            return x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pair<TFirst, TSecond>)) return false;
            return Equals((Pair<TFirst, TSecond>)obj);
        }

        public override int GetHashCode()
        {
            return (Equals(First, null) ? 0 : First.GetHashCode()) ^ (Equals(Second, null) ? 0 : Second.GetHashCode());
        }

        public bool Equals(Pair<TFirst, TSecond> pair)
        {
            return Equals(First, pair.First) && Equals(Second, pair.Second);
        }
    }
}
