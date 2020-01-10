using Prism.Mvvm;

namespace Capibara.Domain.Models
{
    public interface IModel { }

    public abstract class ModelBase<TModel> : BindableBase, IModel where TModel : ModelBase<TModel>
    {
        public abstract TModel Restore(TModel other);
    }
}
