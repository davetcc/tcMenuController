using System;
using System.Threading.Tasks;
using tcMenuControlApi.Protocol;
using embedCONTROL.BaseSerial;
using embedCONTROL.Views;
using tcMenuControlApi.StoreWrapper;
using tcMenuControlApi.Util;

namespace embedCONTROL.Services
{
    public delegate void UIThreadWork();

    /// <summary>
    /// A platform independent way of getting something done on the UI thread
    /// </summary>
    public interface UIThreadMashaller
    {
        Task OnUiThread(UIThreadWork work);
    }

    public class ApplicationContext
    {
        private static object _contextLock = new object();
        private static ApplicationContext _theInstance;

        private volatile ISerialPortFactory _serialPortFactory = null;
        public UIThreadMashaller ThreadMarshaller {get;}

        public PrefsAppSettings AppSettings { get; }
        public IDataStore DataStore { get; }
        public ISerialPortFactory SerialPortFactory => _serialPortFactory;

        public INavigationManager NavigationManager { get; set; }

        public bool IsSerialAvailable => _serialPortFactory != null;

        public SystemClock Clock => new SystemClock();

        public static ApplicationContext Instance
        {
            get
            {
                lock (_contextLock)
                {
                    return _theInstance;
                }
            }
        }

        public LibraryVersion Version { get; }
        public DeploymentType DeploymentType { get; }
        public string AppVersion => $"{Version} ({DeploymentType})";


        public ApplicationContext(UIThreadMashaller marshaller, LibraryVersion version, DeploymentType deploymentType)
        {
            var configDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            Version = version;
            DeploymentType = deploymentType;

            ThreadMarshaller = marshaller;

            AppSettings = new PrefsAppSettings();
            AppSettings.Load(configDir);

            var persistor = new XmlMenuConnectionPersister(configDir);
            DataStore = new ConnectionDataStore(persistor);

            _theInstance = this;
        }


        public void SetSerialFactory(ISerialPortFactory factory)
        {
            _serialPortFactory = factory;
        }
    }
}
