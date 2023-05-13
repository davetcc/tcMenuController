﻿using System;
using System.Threading.Tasks;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.Serialisation;

namespace embedCONTROL.ControlMgr
{
    public abstract class BaseBoolEditorComponent : BaseEditorComponent
    {
        protected bool _currentVal = false;

        protected BaseBoolEditorComponent(IRemoteController controller, ComponentSettings settings, MenuItem item)
            : base(controller, settings, item)
        {
        }

        public override void OnItemUpdated(AnyMenuState newState)
        {
            if (newState is MenuState<bool> boolState)
            {
                _currentVal = boolState.Value;
                MarkRecentlyUpdated(RenderStatus.RecentlyUpdated);
            }
        }
        public override string GetControlText()
        {
            string prefix = "";
            if ((DrawingSettings.DrawMode & RedrawingMode.ShowName) != 0) prefix = _item.Name;

            if ((DrawingSettings.DrawMode & RedrawingMode.ShowValue) == 0) return prefix;

            return prefix + " " + MenuItemFormatter.FormatForDisplay(_item, _currentVal);
        }

        protected async Task ToggleState()
        {
            if (_status == RenderStatus.EditInProgress) return;

            try
            {
                var correlation = await _remoteController.SendAbsoluteChange(_item, !_currentVal);
                EditStarted(correlation);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to send message to {_remoteController.Connector.ConnectionName}");
            }
        }
    }

}
