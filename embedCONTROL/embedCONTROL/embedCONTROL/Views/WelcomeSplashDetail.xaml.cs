using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using embedCONTROL.Services;
using embedCONTROL.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomeSplashDetail : ContentPage
    {
        private static readonly string EmbedControlWebsite = "https://www.thecoderscorner.com/products/apps/embed-control/";
        public string ApplicationName { get; }

        public WelcomeSplashDetail()
        {
            BindingContext = this;
            ApplicationName = "embedCONTROL " + ApplicationContext.Instance.AppVersion;
            InitializeComponent();
        }

        private async void DocumentationButtonClicked(object sender, EventArgs e)
        {
            var didLaunch = await Launcher.TryOpenAsync(EmbedControlWebsite);
            if (!didLaunch)
            {
                await DisplayAlert("URL not launched", "Please navigate to https://www.thecoderscorner.com/", "OK");
            }
        }
    }
}