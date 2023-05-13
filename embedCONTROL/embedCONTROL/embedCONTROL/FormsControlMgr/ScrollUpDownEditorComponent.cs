using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.Serialisation;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class ScrollUpDownEditorComponent : FormsUpDownEditorComponentBase<CurrentScrollPosition>
    {
        public ScrollUpDownEditorComponent(MenuItem item, IRemoteController remote, ComponentSettings settings)
            : base(item, remote, settings)
        {
        }

        public override string GetControlText()
        {
            return _currentVal.Value;
        }


        protected override async void BumpCount(int delta)
        {
            if (_status == RenderStatus.EditInProgress) return;
            try
            {
                var posNow = _currentVal.Position;
                var csp = new CurrentScrollPosition(posNow + delta, "");
                var correlation = await _remoteController.SendAbsoluteChange(_item, csp.ToString());
                EditStarted(correlation);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to send message to {_remoteController.Connector.ConnectionName}");
            }
        }

        public override int CurrentInt => _currentVal.Position;
    }
}
