using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using embedCONTROL.Services;

namespace embedCONTROL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GlobalSettingsDetail : ContentPage
    {
        public PrefsAppSettings LocalSettings { get; }

        public GlobalSettingsDetail(INavigationManager manager)
        {
            LocalSettings = new PrefsAppSettings();
            LocalSettings.CloneSettingsFrom(ApplicationContext.Instance.AppSettings);

            InitializeComponent();

            UpdateAllColors();

            DefaultColumnsField.Text = LocalSettings.DefaultNumColumms.ToString(NumberFormatInfo.InvariantInfo);

            TextColorPicker.NavigationManager = manager;
            ButtonColorPicker.NavigationManager = manager;
            UpdateColorPicker.NavigationManager = manager;
            PendingColorPicker.NavigationManager = manager;
            ErrorColorPicker.NavigationManager = manager;
            HighlightColorPicker.NavigationManager = manager;
            DialogColorPicker.NavigationManager = manager;
        }

        private void UpdateAllColors()
        {
            TextColorPicker.ItemColor = LocalSettings.TextColor;
            ButtonColorPicker.ItemColor = LocalSettings.ButtonColor;
            UpdateColorPicker.ItemColor = LocalSettings.UpdateColor;
            PendingColorPicker.ItemColor = LocalSettings.PendingColor;
            HighlightColorPicker.ItemColor = LocalSettings.HighlightColor;
            ErrorColorPicker.ItemColor = LocalSettings.ErrorColor;
            DialogColorPicker.ItemColor = LocalSettings.DialogColor;
        }

        private void UUIDButton_Clicked(object sender, EventArgs e)
        {
            LocalSettings.UniqueId = Guid.NewGuid().ToString();
            UUIDTextField.Text = LocalSettings.UniqueId;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            var settings = ApplicationContext.Instance.AppSettings;
            settings.CloneSettingsFrom(LocalSettings);

            settings.ButtonColor = ButtonColorPicker.ItemColor;
            settings.ErrorColor = ErrorColorPicker.ItemColor;
            settings.PendingColor = PendingColorPicker.ItemColor;
            settings.UpdateColor = UpdateColorPicker.ItemColor;
            settings.TextColor = TextColorPicker.ItemColor;
            settings.HighlightColor = HighlightColorPicker.ItemColor;
            settings.DialogColor = DialogColorPicker.ItemColor;

            if (int.TryParse(DefaultColumnsField.Text, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out var defaultCols))
            {
                settings.DefaultNumColumms = defaultCols;
            }

            ApplicationContext.Instance.AppSettings.Save();
        }

        private void ResetColorsToDefault(object sender, EventArgs e)
        {
            LocalSettings.SetColorsForMode();
            UpdateAllColors();
        }
    }
}