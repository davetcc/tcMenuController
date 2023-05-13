using System;
using System.Collections.Generic;
using System.Linq;
using embedCONTROL.iOS.SerialImpl;
using embedCONTROL.Services;
using Foundation;
using ImageIO;
using Serilog;
using tcMenuControlApi.StoreWrapper;
using tcMenuControlApi.Util;
using UIKit;
using Xamarin.Forms;

namespace embedCONTROL.iOS
{
    
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public LibraryVersion MyVersion { get; private set; }

        public static readonly DeploymentType DeploymentMode = DeploymentType.Beta;
        
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.NSLog()
                .CreateLogger();
            
            var rawText = NSBundle.MainBundle.InfoDictionary.ValueForKey((NSString) "CFBundleShortVersionString");
            MyVersion = new LibraryVersion(rawText?.ToString() ?? "0.0.0");

            ApplicationContext appCtx = new ApplicationContext(new XamarinFormsMarshaller(), MyVersion, DeploymentMode);
            appCtx.SetSerialFactory(new AppleBluetoothSerialFactory());

            LoadApplication(new App());
            
            return base.FinishedLaunching(app, options);
        }
    }
}
