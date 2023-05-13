using System;
using System.Collections.Generic;
using System.Text;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.Protocol;
using tcMenuControlApi.RemoteCore;

namespace tcMenuControlApi.SocketRemote
{

    public class SocketRemoteControllerBuilder : RemoteControllerBuilderBase<SocketRemoteControllerBuilder>
    {
        private string _host;
        private int _port;

        public SocketRemoteControllerBuilder WithHostAndPort(string host, int port)
        {
            _host = host;
            _port = port;
            return this;
        }

        public override SocketRemoteControllerBuilder GetThis()
        {
            return this;
        }

        public override IRemoteConnector BuildConnector(bool pairing)
        {
            return new SocketRemoteConnector(new LocalIdentification(_localGuid, _localName), _host, _port, GetDefaultConverters(), _protocol, _clock, pairing);
        }
    }
}
