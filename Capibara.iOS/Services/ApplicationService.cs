using System;
using Capibara.Services;

namespace Capibara.iOS.Services
{
    public class ApplicationService : IApplicationService
    {
        void IApplicationService.Exit() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

        string IApplicationService.StoreUrl => string.Empty;
    }
}
