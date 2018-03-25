using System;

namespace Capibara.Services
{
    public interface IApplicationService 
    {
        void Exit();

        string StoreUrl { get; }

        string Platform { get; }

        string AppVersion { get; }
    }
}

