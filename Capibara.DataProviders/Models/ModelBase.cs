using Prism.Mvvm;
using Unity;
using Unity.Attributes;

namespace Capibara.Models
{
    public class ModelBase<TModel> : BindableBase where TModel : ModelBase<TModel>
    {
        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        [Dependency]
        public Net.IRequestFactory RequestFactory { get; set; }

        [Dependency]
        public Net.IChannelFactory ChannelFactory { get; set; }

        public virtual void Restore(TModel model) { }
    }
}
