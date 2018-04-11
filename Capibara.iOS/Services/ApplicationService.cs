using System;
using Capibara.Services;
using Foundation;
using Security;
using UIKit;

namespace Capibara.iOS.Services
{
    public class ApplicationService : IApplicationService
    {
        private string uuid = string.Empty;

        void IApplicationService.Exit() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

        string IApplicationService.StoreUrl => "https://itunes.apple.com/jp/app/capibara/id1367296814?l=ja&ls=1&mt=8";

        string IApplicationService.Platform => "iOS";

        string IApplicationService.AppVersion => NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();

        string IApplicationService.UUID
        {
            get
            {
                if (!this.uuid.IsNullOrEmpty())
                    return this.uuid;

                // SecKind.GenericPasswordの時、AccountとServiceの２つでもってレコードがユニークになる
                using (SecRecord record = new SecRecord(SecKind.GenericPassword))
                {
                    record.Account = NSBundle.MainBundle.BundleIdentifier.ToLower();
                    record.Service = NSBundle.MainBundle.BundleIdentifier.ToLower();

                    SecStatusCode statusCode;
                    var match = SecKeyChain.QueryAsRecord(record, out statusCode);
                    if (statusCode == SecStatusCode.Success)
                    {
                        this.uuid = match.Generic.ToString();
                    }
                    else
                    {
                        this.uuid = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                        using (NSData pass = NSData.FromString(uuid))
                        {
                            record.Generic = pass;
                            record.Accessible = SecAccessible.Always;
                            SecKeyChain.Add(record);
                        }
                    }
                }

                return this.uuid;
            }
        }
    }
}
