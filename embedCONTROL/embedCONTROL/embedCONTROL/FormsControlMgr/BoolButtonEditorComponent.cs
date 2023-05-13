using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class BoolButtonEditorComponent : BaseBoolEditorComponent
    {
        private Button _button;

        public BoolButtonEditorComponent(MenuItem item, IRemoteController remote, ComponentSettings settings)
            : base(remote, settings, item)
        {
        }

        public View CreateComponent()
        {
            _button = new Button
            {
                Text = _item.Name,
                BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            if (_item.ReadOnly)
            {
                _button.IsEnabled = false;
            }
            else
            {
                _button.Clicked += Button_Click;
            }

            return _button;
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            await ToggleState();
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            _button.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _button.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _button.Text = str;
        }
    }

}
