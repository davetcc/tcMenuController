using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class IntegerUpDownEditorComponent : FormsUpDownEditorComponentBase<int>
    {
        public IntegerUpDownEditorComponent(MenuItem item, IRemoteController remote, ComponentSettings settings)
            : base(item, remote, settings)
        {
        }

        public override int CurrentInt => _currentVal;
    }
}
