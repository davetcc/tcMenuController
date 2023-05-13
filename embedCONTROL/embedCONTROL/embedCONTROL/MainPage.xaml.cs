using System;
using System.Collections.Generic;
using embedCONTROL.Services;
using embedCONTROL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{
    public interface INavigationManager
    {
        void NavigateToPage(Page theNewPage, bool pushed = false);
        void PushPageOn(Page theNewPage);
        void PopPage();
        void NavigateToConnectionNamed(string name);
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmbedControlMainPage : MasterDetailPage, INavigationManager
    {
        private readonly Stack<Page> _pageStack = new Stack<Page>(10);
        private Page _currentPage;
        private MainPageMasterViewModel _model;

        public EmbedControlMainPage()
        {
            InitializeComponent();
            _model = new MainPageMasterViewModel();
            BindingContext = _model;

            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            ApplicationContext.Instance.NavigationManager = this;
        }

        public void NavigateToConnectionNamed(string name)
        {
            NavigateToPage(_model.GetConnectionPage(name, this));
        }

        public void NavigateToPage(Page thePage, bool pushed = false)
        {
            // if there's a non pushed page then we clear the stack.
            if(!pushed) _pageStack.Clear();

            _currentPage = thePage;
            var navPage = new NavigationPage(thePage);
            Detail = navPage;
            IsPresented = false;
        }

        public void PushPageOn(Page theNewPage)
        {
            _pageStack.Push(_currentPage);
            NavigateToPage(theNewPage, true);
        }

        public void PopPage()
        {
            if (_pageStack.Count > 0)
            {
                NavigateToPage(_pageStack.Pop());
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is IEmbedControlPageItem item))return;

            var page = item.GetNavigationItem(this);
            NavigateToPage(page);

            MasterPage.ListView.SelectedItem = null;
        }
    }
}