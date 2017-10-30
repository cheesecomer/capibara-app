﻿using System;

using Microsoft.Practices.Unity;

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
        public ISecureIsolatedStorage SecureIsolatedStorage { get; set; }

        public virtual void Restore(TModel model)
        {

        }
    }
}
