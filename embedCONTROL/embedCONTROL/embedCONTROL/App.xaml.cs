using System;
using embedCONTROL.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL
{
    public partial class App : Application
    {
        public App()
        {
            Device.SetFlags(new[] {"RadioButton_Experimental"});
            InitializeComponent();

            MainPage = new EmbedControlMainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
