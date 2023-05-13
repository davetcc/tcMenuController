using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class TextFieldEditorComponent<TVal> : FormsEditorBase<TVal>
    {
        private Entry _textEntry;

        public TextFieldEditorComponent(IRemoteController remote, ComponentSettings settings, MenuItem item)
            : base(remote, settings, item)
        {
        }

        public View CreateComponent()
        {
            _textEntry = new Entry
            {
                Text = GetControlText(),
                BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
                TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
                IsEnabled = !_item.ReadOnly
            };

            return MakeTextComponent(_textEntry, IsItemEditable(_item), SendButton_Clicked);
        }


        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            await ValidateAndSend(_textEntry.Text).ConfigureAwait(true);
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            _textEntry.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
            _textEntry.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
            _textEntry.Text = str;
        }
    }

}
