using Android.Content;
using AppTailler.Droid;
using System.Runtime.Remoting.Contexts;
using Xamarin.Forms;
using static Android.Resource;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace AppTailler.Droid
{
    public class AndroidDevice : IDevice
    {
        private Android.Telephony.TelephonyManager mTelephonyMgr;
        public string GetIMEI()
        {
            return "";
        }

    }
}