using System;
namespace Capibara
{
    public interface IApplication
    {
        IEnvironment Environment { get; }
    }
}
