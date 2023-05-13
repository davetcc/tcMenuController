using System;
using System.Globalization;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class DateFieldEditorComponent : FormsEditorBase<string>
    {
        private DatePicker _dateField;

        public DateFieldEditorComponent(IRemoteController remote, ComponentSettings settings, MenuItem item)
            : base(remote, settings, item)
        {
        }

        public View CreateComponent()
        {
            _dateField = new DatePicker {Date = DateTime.Now, IsEnabled = !_item.ReadOnly,};

            return MakeTextComponent(_dateField, true, DateSendToRemote);
        }

        private async void DateSendToRemote(object sender, EventArgs e)
        {
            var dateStr = _dateField.Date.ToString("yyyy/MM/dd");
            await ValidateAndSend(dateStr).ConfigureAwait(true);
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            if (DateTime.TryParseExact(str, "yyyy/MM/dd",CultureInfo.InvariantCulture, DateTimeStyles.None, out var theDate))
            {
                _dateField.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
                _dateField.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
                _dateField.Date = theDate;
            }
        }
    }

}
