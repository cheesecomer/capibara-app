using Prism.Mvvm;

namespace Capibara.Domain.Models
{
    public abstract class ModelBase<TModel> : BindableBase where TModel : ModelBase<TModel>
    {
        public abstract TModel Restore(TModel other);
    }
}
