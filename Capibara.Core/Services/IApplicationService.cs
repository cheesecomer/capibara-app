using System;

namespace Capibara.Services
{
    public interface IApplicationService 
    {
        void Exit();

        string StoreUrl { get; }
    }
}

