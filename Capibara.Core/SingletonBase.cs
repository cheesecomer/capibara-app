namespace Capibara
{
    /// <summary>
    /// シングルトンの基底クラス。
    /// </summary>
    public class SingletonBase<T> where T : SingletonBase<T>, new()
    {
        private static readonly object lockObject = new object();

        private static T instance = default(T);

        /// <summary>
        /// インスタンスを取得します
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (SingletonBase<T>.lockObject)
                {
                    if (SingletonBase<T>.instance == null)
                    {
                        SingletonBase<T>.instance = new T();
                        SingletonBase<T>.instance.Init();
                    }

                    return SingletonBase<T>.instance;
                }
            }
        }

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        public static void Clear()
        {
            lock (SingletonBase<T>.lockObject)
            {
                if (SingletonBase<T>.instance != null)
                {
                    SingletonBase<T>.instance.Destroy();
                }

                SingletonBase<T>.instance = default(T);
            }
        }

        public virtual void Init() { }

        public virtual void Destroy() { }
    }
}
