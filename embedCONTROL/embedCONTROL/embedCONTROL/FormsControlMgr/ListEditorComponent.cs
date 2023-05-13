using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using embedCONTROL.ControlMgr;
using embedCONTROL.Services;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class ListEditorComponent : BaseEditorComponent
    {
        private readonly RuntimeListStringAdapter _stringAdapter;
        private readonly ObservableCollection<string> _actualData = new ObservableCollection<string>();
        private ListView _listView;

        public ListEditorComponent(IRemoteController remote, ComponentSettings settings, MenuItem item, 
            RuntimeListStringAdapter adapter = null)
            : base(remote, settings, item)
        {
            _stringAdapter = adapter;
        }

        public override void ChangeControlSettings(RenderStatus status, string text)
        {
            _listView.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.HIGHLIGHT).AsXamarin();
        }

        public View CreateComponent()
        {
            if (_item is RuntimeListMenuItem listItem)
            {
                _listView = new ListView
                {
                    ItemsSource = _actualData,
                    BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.HIGHLIGHT).AsXamarin(),
                    HeightRequest = 20 * listItem.InitialRows,
                };
                return _listView;

            }
            else return new Label{Text=$"item {_item} not a list"};
        }

        public override string GetControlText()
        {
            return null;
        }

        public override void OnItemUpdated(AnyMenuState newValue)
        {
            if(newValue is MenuState<List<string>> listState) UpdateAll(listState.Value);
        }

        private void UpdateAll(List<string> values)
        {
            ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                _actualData.Clear();
                foreach (var val in values)
                {
                    var item = (_stringAdapter != null) ? _stringAdapter.Invoke(val) : val;
                    _actualData.Add(item);
                }
            });
        }
    }

}
