using System;
using embedCONTROL.ControlMgr;
using embedCONTROL.FormsControlMgr;
using embedCONTROL.Models;
using embedCONTROL.Services;
using embedCONTROL.Utils;
using embedCONTROL.ViewModels;
using tcMenuControlApi.Commands;
using tcMenuControlApi.RemoteStates;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{

    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TcConnectionPage : ContentPage, IDialogViewer
    {
        public TcMenuConnection Connection { get; }
        private readonly INavigationManager _navigationManager;
        private volatile XamarinFormsScreenManager _screenMgr;
        private volatile TreeComponentManager _controlManager;
        private MenuButtonType _button1Mode = MenuButtonType.NONE, _button2Mode = MenuButtonType.NONE;

        public TcConnectionPage(TcMenuConnection connection, INavigationManager navMgr)
        {
            _navigationManager = navMgr;

            Connection = connection;
            Connection.ConnectIfNeeded();

            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = 35 });
            }

            CreateDrawingComponents();
        }

        private void CreateDrawingComponents()
        {
            Connection.Controller.Connector.ConnectionChanged += AuthenticationStatusHasChanged;
            var appSettings = ApplicationContext.Instance.AppSettings;
            _screenMgr = new XamarinFormsScreenManager(Connection.Controller, ItemLayoutArea, appSettings.DefaultNumColumms);
            _controlManager = new TreeComponentManager(_screenMgr, Connection, appSettings, this);
        }

        private async void AuthenticationStatusHasChanged(AuthenticationStatus status)
        {
            await ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                if (status == AuthenticationStatus.FAILED_AUTH)
                {
                    Connection.CompletelyDisconnect();
                    _navigationManager.PushPageOn(new DevicePairingDetail(Connection, OnPairingFinished));
                }
                else
                {
                    var con = Connection.Controller?.Connector;
                    if (con != null)
                    {
                        Title = Connection.Name + " - " + EasyReadableAuthStatus(con.AuthStatus);
                        RemoteDeviceInfo.Text = con.RemoteInfo?.ToString() ?? "Not available";
                        ConnectionStatus.Text = con.AuthStatus.ToString().CapsUnderscoreToReadable();
                    }
                }
            });
        }

        private string EasyReadableAuthStatus(AuthenticationStatus conAuthStatus)
        {
            switch (conAuthStatus)
            {
                case AuthenticationStatus.NOT_STARTED:
                    return "Off";
                case AuthenticationStatus.AWAITING_CONNECTION:
                    return "Disconnected";
                case AuthenticationStatus.ESTABLISHED_CONNECTION:
                    return "Connected";
                case AuthenticationStatus.SEND_JOIN:
                    return "Joining";
                case AuthenticationStatus.FAILED_AUTH:
                    return "Disallowed";
                case AuthenticationStatus.AUTHENTICATED:
                    return "Authorized";
                case AuthenticationStatus.BOOTSTRAPPING:
                    return "Bootstrap";
                case AuthenticationStatus.CONNECTION_READY:
                    return "Ready";
                default:
                    throw new ArgumentOutOfRangeException(nameof(conAuthStatus), conAuthStatus, null);
            }
        }

        private void OnPairingFinished(bool obj)
        {
            _navigationManager.PopPage();
            Connection.ConnectIfNeeded();

            CreateDrawingComponents();
        }

        private void EditConnectionBtn_Click(object sender, EventArgs e)
        {
            Connection.ForceReconnect();
        }

        private async void RemoveConnectionBtn_Click(object sender, EventArgs e)
        {
            var result = await DisplayActionSheet("Really delete " + Connection.Name + " permanently", 
                "No", "Yes");
            if (result == "Yes")
            {
                Connection.CompletelyDisconnect();
                await ApplicationContext.Instance.DataStore.DeleteItemAsync(Connection.LocalId);
                _navigationManager.PushPageOn(new WelcomeSplashDetail());
            }
        }

        public void SetButton1(MenuButtonType buttonType)
        {
            DialogButton1.IsEnabled = buttonType != MenuButtonType.NONE;
            DialogButton1.Text = FriendlyButtonType(buttonType);
            _button1Mode = buttonType;
        }

        private string FriendlyButtonType(MenuButtonType btnType)
        {
            switch (btnType)
            {
                case MenuButtonType.NONE:
                    return "";
                case MenuButtonType.OK:
                    return "OK";
                case MenuButtonType.ACCEPT:
                    return "Accept";
                case MenuButtonType.CANCEL:
                    return "Cancel";
                case MenuButtonType.CLOSE:
                    return "Close";
                default:
                    throw new ArgumentOutOfRangeException(nameof(btnType), btnType, null);
            }
        }

        public void SetButton2(MenuButtonType buttonType)
        {
            DialogButton2.IsEnabled = buttonType != MenuButtonType.NONE;
            DialogButton2.Text = FriendlyButtonType(buttonType);
            _button2Mode = buttonType;
        }

        public void Show(bool visible)
        {
            var settings = ApplicationContext.Instance.AppSettings;
            DialogButton1.BackgroundColor = settings.ButtonColor.Bg.AsXamarin();
            DialogButton1.TextColor = settings.ButtonColor.Fg.AsXamarin();
            DialogButton2.BackgroundColor = settings.ButtonColor.Bg.AsXamarin();
            DialogButton2.TextColor = settings.ButtonColor.Fg.AsXamarin();
            DialogStack.BackgroundColor = settings.DialogColor.Bg.AsXamarin();
            DialogTitle.TextColor = settings.DialogColor.Fg.AsXamarin();
            DialogText.TextColor = settings.DialogColor.Fg.AsXamarin();
            
            DialogStack.IsVisible = visible;
        }

        public void SetText(string title, string subject)
        {
            DialogTitle.Text = title;
            DialogText.Text = subject;
        }

        private async void DialogButton1_OnClicked(object sender, EventArgs e)
        {
            await Connection.Controller.SendDialogAction(_button1Mode);
        }

        private async void DialogButton2_OnClicked(object sender, EventArgs e)
        {
            await Connection.Controller.SendDialogAction(_button2Mode);
        }
    }
}