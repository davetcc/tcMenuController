using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class TimeFieldEditorComponent : FormsEditorBase<string>
    {
        private TimePicker _timeField;

        public TimeFieldEditorComponent(IRemoteController remote, ComponentSettings settings, MenuItem item)
            : base(remote, settings, item)
        {
        }

        private string GetFormat()
        {
            if (_item is EditableTextMenuItem timeItem)
            {
                return (timeItem.EditType == EditItemType.TIME_12H) ? "hh:mm:sstt" :
                    (timeItem.EditType == EditItemType.TIME_24H) ? "HH:mm:ss" : "HH:mm:ss.ff";
            }

            return "T";
        }

        public View CreateComponent()
        {
            if (_item is EditableTextMenuItem timeItem)
            {
                _timeField = new TimePicker {Format = GetFormat(), IsEnabled = !_item.ReadOnly,};

                return MakeTextComponent(_timeField, !_item.ReadOnly, TimeComponentSend);
            }

            throw new ArgumentException($"{_item} is not a time item");
        }

        private async void TimeComponentSend(object sender, EventArgs e)
        {
            var theTime = DateTime.Now + _timeField.Time;
            await ValidateAndSend(theTime.ToString("HH:mm:ss")).ConfigureAwait(true);
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            _timeField.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
            _timeField.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();

            var strippedStr = str.Replace("[", ""); 
            strippedStr = strippedStr.Replace("]", ""); 

            if (DateTime.TryParseExact(strippedStr, GetFormat(), CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var theTime))
            {
                _timeField.Time = theTime.TimeOfDay;
            }
        }
    }
}
