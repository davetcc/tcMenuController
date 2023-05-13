using System;
using embedCONTROL.ControlMgr;
using embedCONTROL.Services;
using embedCONTROL.Views;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.Serialisation;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class RgbColorEditorComponent : FormsEditorBase<PortableColor>
    {
        private Button _colorButton;

        public RgbColorEditorComponent(MenuItem item, IRemoteController remote, ComponentSettings settings)
            : base(remote, settings, item)
        {
        }

        public View CreateComponent()
        {
            _colorButton = new Button()
            {
                BackgroundColor = Color.Gray
            };

            if (IsItemEditable(_item))
            {
                _colorButton.Clicked += Button_Click;
            }

            return MakeTextComponent(_colorButton, false);
        }


        private void Button_Click(object sender, EventArgs e)
        {
            var navigator = ApplicationContext.Instance.NavigationManager;
            var dialog = new ColorDialog(CurrentVal.AsXamarin(), _item.Name, navigator, ColorChangeNotify);
            navigator.PushPageOn(dialog);
        }

        private async void ColorChangeNotify(Color newColor)
        {
            await ValidateAndSend(newColor.ToPortable().ToString());
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            _colorButton.BackgroundColor = CurrentVal.AsXamarin();
            _colorButton.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _colorButton.Text = str;
        }
    }

}
