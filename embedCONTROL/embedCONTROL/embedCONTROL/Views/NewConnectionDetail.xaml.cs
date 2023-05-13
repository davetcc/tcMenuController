using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using embedCONTROL.BaseSerial;
using embedCONTROL.Models;
using embedCONTROL.Services;
using embedCONTROL.Simulator;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewConnectionDetail : ContentPage
    {
        private readonly INavigationManager _manager;

        public NewConnectionDetail(INavigationManager manager)
        {
            _manager = manager;
            InitializeComponent();
            if (!ApplicationContext.Instance.IsSerialAvailable)
            {
                SerialPortCheckbox.IsEnabled = false;
            }
        }

        private async void CreateButton_OnClicked(object sender, EventArgs e)
        {
            if ((ConnectionNameEntry.Text?.Length ?? 0)==0)
            {
                await DisplayAlert("Name is missing", "Please provide a connection name", "Close");
                return;
            }

            IConnectionConfiguration connectionData = null;
            if (ManualSocketCheckbox.IsChecked)
            {
                var ip = IpAddressHostEntry.Text;
                var port = IpAddressPortEntry.Text;
                if (ip?.Length != 0 && port?.Length != 0 && ushort.TryParse(port, NumberStyles.Integer, CultureInfo.InvariantCulture, out var actPort))
                {
                    connectionData = new RawSocketConfiguration(ip, actPort);
                }
                else
                {
                    await DisplayAlert("Invalid IP or port", "Please ensure you provide a host or IP address and a numeric port", "OK");
                    return;
                }
            }
            else if (SimulatorCheckbox.IsChecked)
            {
                connectionData = new SimulatorConfiguration()
                {
                    JsonObjects = JsonDataEditor.Text?.Trim() ?? ""
                };
                
            }
            else if (SerialPortCheckbox.IsChecked)
            {
                if (BaudRatePicker.SelectedIndex != -1 && SerialPortPicker.SelectedItem is SerialPortInformation spi)
                {
                    connectionData = new SerialCommsConfiguration(spi, BaudRatePicker.SelectedItem as int? ?? 115200);
                }
                else
                {
                    await DisplayAlert("Serial settings incorrect", "Please choose a serial port and baud rate", "OK");
                    return;
                }
            }

            if(connectionData == null)
            {
                await DisplayAlert("Internal error", "Unable to create a connection with the selected parameters", "OK");
                return;
            }

            var name = ConnectionNameEntry.Text;
            var connection = new TcMenuConnection {ConnectionConfig = connectionData, Name =  name};
            await ApplicationContext.Instance.DataStore.AddOrUpdateItemAsync(connection).ConfigureAwait(true);

            _manager.NavigateToConnectionNamed(name);
        }

        private void SerialPortCheckbox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var sf = ApplicationContext.Instance.SerialPortFactory;

            if (SerialPortCheckbox.IsChecked)
            {
                SerialPortPicker.ItemsSource = new ObservableCollection<SerialPortInformation>();
                var scanStarted = sf.StartScanningPorts(SerialPortType.ALL, portInfo =>
                {
                    ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
                    {
                        SerialPortPicker?.ItemsSource.Add(portInfo);
                    });
                });

                if (scanStarted)
                {
                    BaudRatePicker.ItemsSource = SerialPortInformation.ALL_BAUD_RATES.ToList();
                    SerialPortPicker.IsEnabled = true;
                    BaudRatePicker.IsEnabled = true;
                    BaudRatePicker.SelectedItem = 115200;
                }
                else
                {
                    DisplayAlert("Bluetooth not supported", "Cannot scan for bluetooth devices", "OK");
                }
            }
            else
            {
                sf.StopScanningPorts();
                SerialPortPicker.ItemsSource?.Clear();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ApplicationContext.Instance.SerialPortFactory?.StopScanningPorts();
        }
    }
}