using System;
using System.ComponentModel;
using embedCONTROL.Services;
using embedCONTROL.Utils;
using Serilog;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.RemoteStates;

namespace embedCONTROL.Models
{

    /// <summary>
    /// This represents a connection to an embeded device. It holds the details about how to connect
    /// and all the required components to initiate a connection.
    /// </summary>
    public class TcMenuConnection : INotifyPropertyChanged
    {
        private ILogger logger = Log.Logger.ForContext<TcMenuConnection>();

        /// <summary>
        /// The remote controller itself.
        /// </summary>
        private IRemoteController _remoteController = null;

        /// <summary>
        /// Protects the _remoteController instance
        /// </summary>
        private readonly object _remoteLock = new object();

        /// <summary>
        /// Gets (or creates) the remote controller associated with this connection object.
        /// </summary>
        public IRemoteController Controller
        {
            get
            {
                lock (_remoteLock)
                {
                    return _remoteController;
                }
            }
        }
                 

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A connection configuration that represents the method by which to obtain a connection.
        /// Usually constructed based on the connection type.
        /// </summary>
        public IConnectionConfiguration ConnectionConfig { get; set; }

        /// <summary>
        /// The unique Id of this menu
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// The local ID that is used to store the item locally
        /// </summary>
        public int LocalId { get; set; }

        /// <summary>
        /// The user friendly name given to the connection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of connection, defined by a ConnectionType
        /// </summary>
        public string Type { get => ConnectionConfig?.Name; }

        /// <summary>
        /// The connection status based on the underlying connection.
        /// </summary>
        public string ConnectionInformation { get; set; } = "Not yet connected";

        /// <summary>
        /// Create an empty menu connection object ready to be populated.
        /// </summary>
        public TcMenuConnection()
        {
            UniqueId = Guid.NewGuid();
            Name = "Unknown";
            ConnectionConfig = new RawSocketConfiguration();
            LocalId = -1;
        }

        /// <summary>
        /// Create a menu connection object with known parameters.
        /// </summary>
        /// <param name="uniqueKey">the unique key of this connection</param>
        /// <param name="connectionName">the connection name associated with this connection</param>
        /// <param name="connectionConfiguration">the configuration for the connection</param>
        public TcMenuConnection(Guid uniqueKey, string connectionName, IConnectionConfiguration contype, int localId)
        {
            UniqueId = Guid.NewGuid();
            Name = connectionName;
            ConnectionConfig = contype;
            LocalId = localId;
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void RemoteControl_AuthChanged(AuthenticationStatus status)
        {
            ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                if (Controller == null || Controller.Connector == null)
                {
                    ConnectionInformation = "Not yet connected";
                    OnPropertyChanged("ConnectionInformation");
                    return;
                }

                string who;
                if (Controller.Connector.RemoteInfo != null)
                {
                    who = Controller.Connector.RemoteInfo.Name + '(' + Controller.Connector.RemoteInfo.Platform + ')';
                }
                else who = Controller.Connector.ConnectionName;
                ConnectionInformation = who + ' ' + status.ToString().CapsUnderscoreToReadable();
                OnPropertyChanged("ConnectionInformation");
            });
        }

        public void ConnectIfNeeded()
        {
            lock (_remoteLock)
            {
                if(_remoteController == null)
                {
                    logger.Information("Connecting to " + Name);
                    _remoteController = ConnectionConfig.Build();
                    _remoteController.Connector.ConnectionChanged += RemoteControl_AuthChanged;
                }
            }
        }

        public void CompletelyDisconnect()
        {
            lock(_remoteLock)
            {
                logger.Information("Completely disconnecting from " + Name);
                if (_remoteController == null) return;
                _remoteController.Stop(false);
                _remoteController.Connector.ConnectionChanged -= RemoteControl_AuthChanged;
                _remoteController = null;


                ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
                {
                    ConnectionInformation = "Not yet connected";
                    OnPropertyChanged(nameof(ConnectionInformation));
                });
            }
        }

        public void ForceReconnect()
        {
            _remoteController.Connector.Close();
        }
    }
}