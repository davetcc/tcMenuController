using System;
using tcMenuControlApi.Protocol;
using tcMenuControlApi.RemoteCore;
using embedCONTROL.BaseSerial;
using embedCONTROL.Services;

namespace embedCONTROL.Models
{
    public class SerialCommsConfiguration : IConnectionConfiguration
    {
        public const string SERIAL_PORT_NAME = "Bluetooth / Serial";

        public string Name => $"{SerialInfo?.Id ?? ""}({SerialInfo?.Name ?? ""})@{BaudRate}";

        public SerialPortInformation SerialInfo { get; set; }

        public int BaudRate { get; set; }

        public SerialCommsConfiguration() : this(SerialPortInformation.EMPTY, 115200)
        {
        }

        public SerialCommsConfiguration(SerialPortInformation portInfo, int baud)
        {
            SerialInfo = portInfo;
            BaudRate = 115200;
        }

        public IRemoteController Build()
        {
            var appSettings = ApplicationContext.Instance.AppSettings;

            return new SerialPortControllerBuilder()
                .WithSerialPortAndBaud(SerialInfo, BaudRate)
                .WithNameAndGuid(appSettings.LocalName, Guid.Parse(appSettings.UniqueId))
                .WithProtocol(ProtocolId.TAG_VAL_PROTOCOL)
                .BuildController();
        }

        public bool Pair(PairingUpdateEventHandler handler)
        {
            var appSettings = ApplicationContext.Instance.AppSettings;

            return new SerialPortControllerBuilder()
                .WithSerialPortAndBaud(SerialInfo, BaudRate)
                .WithNameAndGuid(appSettings.LocalName, Guid.Parse(appSettings.UniqueId))
                .WithProtocol(ProtocolId.TAG_VAL_PROTOCOL)
                .PerformPairing(handler);
        }
    }
}
