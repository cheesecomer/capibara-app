using System;
namespace Capibara
{
    public interface ICapibaraApplication
    {
        bool HasPlatformInitializer { get; }
    }
}
