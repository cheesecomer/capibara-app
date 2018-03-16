using System;
using System.Threading.Tasks;

using Unity;
using Unity.Attributes;

namespace Capibara.Net.Channels
{
    public interface IChannel<TMessage>
    {
        event EventHandler Connected;

        event EventHandler Disconnected;

        event EventHandler<TMessage> MessageReceive;

        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [Dependency]
        IUnityContainer Container { get; set; }

        /// <summary>
        /// 環境設定
        /// </summary>
        /// <value>The environment.</value>
        [Dependency]
        IEnvironment Environment { get; set; }

        /// <summary>
        /// セキュア分離ストレージ
        /// </summary>
        /// <value>The secure isolated storage.</value>
        [Dependency]
        IIsolatedStorage IsolatedStorage { get; set; }

        bool IsOpen { get; }

        void Dispose();

        Task<bool> Connect();

        Task Close();
    }
}
