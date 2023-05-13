using System;
using System.Collections.Generic;
using System.Text;
using tcMenuControlApi.Protocol;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.SocketRemote;
using embedCONTROL.Services;

namespace embedCONTROL.Models
{
    public class RawSocketConfiguration : IConnectionConfiguration
    {
        public const string MANUAL_SOCKET_NAME = "Manual Socket";

        public string Name { get; } = MANUAL_SOCKET_NAME;
        public string Host { get; set; }
        public int Port { get; set; }

        public RawSocketConfiguration()
        {
            Host = "localhost";
            Port = 3333;
        }

        public RawSocketConfiguration(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public IRemoteController Build()
        {
            var appSettings = ApplicationContext.Instance.AppSettings;

            return new SocketRemoteControllerBuilder()
                .WithHostAndPort(Host, Port)
                .WithNameAndGuid(appSettings.LocalName, Guid.Parse(appSettings.UniqueId))
                .WithProtocol(ProtocolId.TAG_VAL_PROTOCOL)
                .BuildController();
        }

        public bool Pair(PairingUpdateEventHandler handler)
        {
            var appSettings = ApplicationContext.Instance.AppSettings;

            return new SocketRemoteControllerBuilder()
                .WithHostAndPort(Host, Port)
                .WithNameAndGuid(appSettings.LocalName, Guid.Parse(appSettings.UniqueId))
                .WithProtocol(ProtocolId.TAG_VAL_PROTOCOL)
                .PerformPairing(handler);
        }
    }
}
