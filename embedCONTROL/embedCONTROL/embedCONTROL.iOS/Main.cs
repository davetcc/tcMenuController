using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using embedCONTROL.iOS.SerialImpl;
using embedCONTROL.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace embedCONTROL.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
    internal class XamarinFormsMarshaller : UIThreadMashaller
    {
        public Task OnUiThread(UIThreadWork work)
        {
            return Device.InvokeOnMainThreadAsync(work.Invoke);
        }
    }
}
