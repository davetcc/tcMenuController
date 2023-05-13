using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using embedCONTROL.BaseSerial;
using embedCONTROL.Models;
using embedCONTROL.Simulator;

namespace embedCONTROL.Services
{
    public interface IMenuConnectionPersister
    {
        void Update(TcMenuConnection connection);
        TcMenuConnection LoadNamed(int localId);
        List<TcMenuConnection> LoadAll();
        bool DeleteNamed(int localId);
    }

    public enum PersistedConnectionType
    {
        RAW_SOCKET,
        SERIAL_PORT,
        SIMULATOR
    }

    public class XmlMenuConnectionPersister : IMenuConnectionPersister
    {
        private Serilog.ILogger logger = Serilog.Log.Logger.ForContext<XmlMenuConnectionPersister>();

        public static readonly string EXPECTED_ROOT_NAME = "TcMenuConnection";
        public static readonly string ATTR_NAME = "Name";
        public static readonly string ATTR_UNIQUEID = "UniqueId";
        public static readonly string ATTR_LOCALID = "LocalId";
        public static readonly string CONNECTION_ELEMENT = "Connection";
        public static readonly string CONNECTION_TYPE_ATTR = "ConnectionType";
        public static readonly string CONNTYPE_HOST_ATTRIBUTE = "Host";
        public static readonly string CONNTYPE_PORT_ATTRIBUTE = "Port";
        public static readonly string CONNTYPE_SERTYPE_ATTRIBUTE = "SerialType";
        public static readonly string CONNTYPE_SERNAME_ATTRIBUTE = "SerialName";
        public static readonly string CONNTYPE_SIMNAME_ATTRIBUTE = "SimName";
        public static readonly string CONNTYPE_SERID_ATTRIBUTE = "SerialId";
        public static readonly string CONNTYPE_BAUD_ATTRIBUTE = "Baud";

        private readonly string _baseDirName;

        public XmlMenuConnectionPersister(string baseFileName)
        {
            _baseDirName = baseFileName;
            logger.Information($"Persistor created on {baseFileName}");
        }

        public bool DeleteNamed(int toDelete)
        {
            try
            {
                logger.Information($"Delete on {toDelete}");
                var docPath = ToProperPath(toDelete);
                File.Delete(docPath);
                return true;
            }
            catch(Exception e)
            {
                logger.Error(e, $"Unable to delete the file {toDelete}");
                return false;
            }
        }

        private string ToProperPath(int localId)
        {
            return Path.Combine(_baseDirName, $"connection-{localId}.xml");
        }

        public TcMenuConnection LoadNamed(int localId)
        {
            logger.Information($"Load named {localId}");
            var docPath = ToProperPath(localId);
            return LoadFile(docPath);
        }

        public List<TcMenuConnection> LoadAll()
        {
            logger.Information("Loading all files");
            var list = new List<TcMenuConnection>();
            var allConnections = Directory.EnumerateFiles(_baseDirName, "connection-*.xml");
            foreach(var con in allConnections)
            {
                try
                {
                    list.Add(LoadFile(con));
                }
                catch(Exception ex)
                {
                    logger.Error(ex, $"Unable to load {con}, skipping");
                }
            }
            return list;
        }

        public TcMenuConnection LoadFile(string docPath)
        {
            logger.Information($"Load file {docPath}");

            var doc = XDocument.Load(docPath);
            if (!doc.Root.Name.LocalName.Equals(EXPECTED_ROOT_NAME)) throw new ArgumentException($"{EXPECTED_ROOT_NAME} was not root");
            var name = doc.Root.Attribute(ATTR_NAME)?.Value;
            var uuid = doc.Root.Attribute(ATTR_UNIQUEID)?.Value;
            var localId = doc.Root.Attribute(ATTR_LOCALID)?.Value;

            var connectionType = doc.Root.Elements()
                .Where(el => el.Name.LocalName.Equals(CONNECTION_ELEMENT))
                .First();
            if (connectionType == null) throw new ArgumentException($"{CONNECTION_ELEMENT} not within root");

            var connectionConfig = ProcessConnectionType(connectionType);

            logger.Information($"Creating connection {name}, {uuid} with {connectionType}");

            return new TcMenuConnection
            {
                Name = name,
                UniqueId = Guid.Parse(uuid),
                ConnectionConfig = connectionConfig,
                LocalId = int.Parse(localId)
            };
        }

        private IConnectionConfiguration ProcessConnectionType(XElement connectionType)
        {
            var ty = (PersistedConnectionType) Enum.Parse(typeof(PersistedConnectionType), connectionType.Attribute(CONNECTION_TYPE_ATTR).Value);
            switch (ty)
            {
                case PersistedConnectionType.SERIAL_PORT:
                    return new SerialCommsConfiguration(
                        new SerialPortInformation(
                            connectionType.Attribute(CONNTYPE_SERNAME_ATTRIBUTE).Value,
                            (SerialPortType)Enum.Parse(typeof(SerialPortType), connectionType.Attribute(CONNTYPE_SERTYPE_ATTRIBUTE).Value),
                            connectionType.Attribute(CONNTYPE_SERID_ATTRIBUTE).Value),
                        int.Parse(connectionType.Attribute(CONNTYPE_BAUD_ATTRIBUTE).Value));
                case PersistedConnectionType.SIMULATOR:
                    return new SimulatorConfiguration(
                        connectionType.Attribute(CONNTYPE_SIMNAME_ATTRIBUTE)?.Value ?? "-",
                        connectionType.Value
                    );
                case PersistedConnectionType.RAW_SOCKET:
                default:
                    return new RawSocketConfiguration(
                        connectionType.Attribute(CONNTYPE_HOST_ATTRIBUTE).Value,
                        int.Parse(connectionType.Attribute(CONNTYPE_PORT_ATTRIBUTE).Value)
                    );
            }
        }

        public void Update(TcMenuConnection connection)
        {
            if (connection.LocalId == -1) throw new DataStoreException(DataStoreError.InvalidUniqueId);

            XElement conElem = new XElement(CONNECTION_ELEMENT);
            switch(connection.ConnectionConfig)
            {
                case RawSocketConfiguration socket:
                    conElem.Add(new XAttribute(CONNECTION_TYPE_ATTR, PersistedConnectionType.RAW_SOCKET));
                    conElem.Add(new XAttribute(CONNTYPE_HOST_ATTRIBUTE, socket.Host));
                    conElem.Add(new XAttribute(CONNTYPE_PORT_ATTRIBUTE, socket.Port));
                    break;
                case SerialCommsConfiguration ser:
                    conElem.Add(new XAttribute(CONNECTION_TYPE_ATTR, PersistedConnectionType.SERIAL_PORT));
                    conElem.Add(new XAttribute(CONNTYPE_SERNAME_ATTRIBUTE, ser.SerialInfo?.Name));
                    conElem.Add(new XAttribute(CONNTYPE_SERID_ATTRIBUTE, ser.SerialInfo?.Id));
                    conElem.Add(new XAttribute(CONNTYPE_SERTYPE_ATTRIBUTE, ser.SerialInfo?.PortType));
                    conElem.Add(new XAttribute(CONNTYPE_BAUD_ATTRIBUTE, ser.BaudRate));
                    break;
                case SimulatorConfiguration sc:
                    conElem.Add(new XAttribute(CONNECTION_TYPE_ATTR, PersistedConnectionType.SIMULATOR));
                    conElem.Add(new XAttribute(CONNTYPE_SIMNAME_ATTRIBUTE, sc.Name));
                    conElem.Add(new XCData(sc.JsonObjects));
                    break;
                default:
                    throw new NotSupportedException();
            }

            XDocument document = new XDocument(
                new XElement(EXPECTED_ROOT_NAME,
                    new XAttribute(ATTR_NAME, connection.Name),
                    new XAttribute(ATTR_UNIQUEID, connection.UniqueId),
                    new XAttribute(ATTR_LOCALID, connection.LocalId),
                    conElem
                )
            );
            document.Save(ToProperPath(connection.LocalId));
        }
    }

}
