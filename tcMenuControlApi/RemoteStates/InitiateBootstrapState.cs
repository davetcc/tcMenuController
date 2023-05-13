using System;
using System.Collections.Generic;
using System.Text;
using tcMenuControlApi.Commands;
using tcMenuControlApi.RemoteCore;

namespace tcMenuControlApi.RemoteStates
{
    public class InitiateBootstrapState : BaseRemoteConnectorState
    {
        public InitiateBootstrapState(IRemoteConnectorContext context) : base(context)
        {
        }

        public override bool NeedsRead => true;

        public override AuthenticationStatus AuthStatus => AuthenticationStatus.AUTHENTICATED;

        public override void ProcessCommand(MenuCommand command)
        {
            if (command is HeartbeatCommand hb)
            {
                StandardHeartbeatProcessing(hb);
            }
            else if(command is BootstrapCommand boot)
            {
                if (boot.BootType == BootstrapType.START)
                {
                    Context.ChangeState(AuthenticationStatus.BOOTSTRAPPING);
                }
                else
                {
                    Context.BackToStart("Bootstrap END without START");
                }
            }
        }
    }
}
