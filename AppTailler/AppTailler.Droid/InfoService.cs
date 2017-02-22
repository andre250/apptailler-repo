using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AppTailler.Droid.Services.InfoService))]
namespace AppTailler.Droid.Services
{
    public class InfoService : IInfoService
    {
        public string AppVersionName
        {
            get
            {
                var context = Forms.Context;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
        }

        public int AppVersionCode
        {
            get
            {
                var context = Forms.Context;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
            }
        }
    }
}
