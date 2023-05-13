using embedCONTROL.Models;
using embedCONTROL.Views;
using Xamarin.Forms;

namespace embedCONTROL.ViewModels
{
    public interface IEmbedControlPageItem
    {
        string Name { get; }
        Page GetNavigationItem(INavigationManager navMgr);
    }

    public class TcMenuConnectionPageItem : IEmbedControlPageItem
    {
        private readonly TcMenuConnection _connection;
        private TcConnectionPage _page;

        public string Name => _connection.Name;

        public TcMenuConnectionPageItem(TcMenuConnection connection)
        {
            _connection = connection;
        }

        public Page GetNavigationItem(INavigationManager mgr)
        {
            return _page ?? (_page = new TcConnectionPage(_connection, mgr));
        }

    }

    public class NewConnectionPageItem : IEmbedControlPageItem
    {
        public string Name => "New Connection";

        public Page GetNavigationItem(INavigationManager mgr) => new NewConnectionDetail(mgr);
    }

    public class GlobalSettingsPageItem : IEmbedControlPageItem
    {
        public string Name => "Settings";
        public Page GetNavigationItem(INavigationManager mgr) => new GlobalSettingsDetail(mgr);
    }

    public class AboutPageItem : IEmbedControlPageItem
    {
        public string Name => "About";
        public Page GetNavigationItem(INavigationManager mgr) => new WelcomeSplashDetail();
    }
}
