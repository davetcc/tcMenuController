using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using embedCONTROL.Services;
using embedCONTROL.Views;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.ViewModels
{
    public class MainPageMasterViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IEmbedControlPageItem> MenuItems { get; set; }
        public string ApplicationName => "embedCONTROL " + ApplicationContext.Instance.AppVersion;

        public MainPageMasterViewModel()
        {
            MenuItems = new ObservableCollection<IEmbedControlPageItem>();
            RefreshFromSource();
            ApplicationContext.Instance.DataStore.ChangeNotification += (changed, reordered) => RefreshFromSource();
        }

        public Page GetConnectionPage(string named, INavigationManager mgr)
        {
            var page = MenuItems
                .Where(it => named == it.Name)
                .Select(it => it.GetNavigationItem(mgr))
                .FirstOrDefault() ?? new WelcomeSplashDetail();

            return page;
        }

        private async void RefreshFromSource()
        {
            MenuItems.Clear();
            var items = await ApplicationContext.Instance.DataStore.GetItemsAsync();
            foreach (var item in items)
            {
                MenuItems.Add(new TcMenuConnectionPageItem(item));
            }

            MenuItems.Add(new NewConnectionPageItem());
            MenuItems.Add(new GlobalSettingsPageItem());
            MenuItems.Add(new AboutPageItem());
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
