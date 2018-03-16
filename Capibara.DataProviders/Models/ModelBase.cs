using System;

using Unity;
using Unity.Attributes;

using Prism.Mvvm;

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

        public virtual void Restore(TModel model)
        {

        }
    }
}
