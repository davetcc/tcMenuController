using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using embedCONTROL.Services;
using Serilog;
using tcMenuControlApi.StoreWrapper;
using tcMenuControlApi.Util;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Environment = System.Environment;

namespace embedCONTROL.Droid
{
    [Activity(Label = "embedCONTROL", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private DeploymentType _deploymentType = DeploymentType.Beta;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var logDir= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "/embed.log");
            
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logDir,
                    fileSizeLimitBytes: 1024 * 1024, rollOnFileSizeLimit: true,  // 2mb in total on device
                    retainedFileCountLimit: 2,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Logger = logger;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            
            var version = LibraryVersion.ERROR_VERSION;
            
            var appCtx = new ApplicationContext(new XamarinFormsMarshaller(), version, _deploymentType);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        internal class XamarinFormsMarshaller : UIThreadMashaller
        {
            public Task OnUiThread(UIThreadWork work)
            {
                return Device.InvokeOnMainThreadAsync(work.Invoke);
            }
        }
    }
}