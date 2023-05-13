using System;
using System.Threading.Tasks;
using embedCONTROL.Models;
using embedCONTROL.Services;
using Serilog;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePairingDetail : ContentPage
    {
        private ILogger logger = Log.Logger.ForContext<DevicePairingDetail>();

        private readonly Action<bool> _pairingComplete;
        private readonly TcMenuConnection _connection;
        private volatile bool _pairingDone;

        public DevicePairingDetail(TcMenuConnection theConnection, Action<bool> pairingCompleted)
        {
            _pairingComplete = pairingCompleted;
            _connection = theConnection;
            var myName = ApplicationContext.Instance.AppSettings.LocalName;
            
            Title = "Pairing between " + myName + " and " + theConnection.Name;
            InitializeComponent();
        }


        private async void PairingButton_OnClicked(object sender, EventArgs e)
        {
            if (_pairingDone)
            {
                logger.Debug("Pairing button clicked, exiting as complete");
                _pairingComplete?.Invoke(true);

                return;
            }

            logger.Debug("Pairing button clicked, starting work");

            PairingButton.IsEnabled = false;

            try
            {
                await Task.Run(() =>
                {
                    _connection.ConnectIfNeeded();
                    _connection.ConnectionConfig.Pair(PairingUpdate_Handler);
                });
            }
            catch (Exception) { }

            logger.Debug("Pairing end.");

            PairingButton.IsEnabled = true;
        }

        private void PairingUpdate_Handler(PairingState status)
        {
            logger.Debug($"Pairing updated to {status}");

            _pairingDone = status == PairingState.ACCEPTED;
            ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                PairingStatusField.Text = NicelyFormattedStatus(status);
                if (_pairingDone) PairingButton.Text = "Dismiss, pairing succeeded";
            });

        }

        private static string NicelyFormattedStatus(PairingState status)
        {
            switch (status)
            {
                case PairingState.PAIRING_SENT:
                    return "Sent request";
                case PairingState.NOT_ACCEPTED:
                    return "Request not accepted";
                case PairingState.ACCEPTED:
                    return "Request accepted";
                case PairingState.TIMED_OUT:
                    return "Timed out";
                default:
                    return "Not connected";
            }
        }
    }
}