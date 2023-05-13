using System;
using embedCONTROL.ControlMgr;
using embedCONTROL.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace embedCONTROL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormsFgBgColorPicker : ContentView
    {
        public static BindableProperty FieldNameProperty = BindableProperty.Create(
            propertyName: nameof(FieldName),
            returnType: typeof(string),
            declaringType: typeof(ContentView),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: FieldNamePropertyChanged);

        private static void FieldNamePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            ((FormsFgBgColorPicker)bindable).FieldName = newValue.ToString();
        }


        public string FieldName
        {
            get => (string)base.GetValue(FieldNameProperty);
            set => base.SetValue(FieldNameProperty, value);
        }

        public INavigationManager NavigationManager { get; set; }

        private ControlColor _itemColor;
        public ControlColor ItemColor
        {
            get => _itemColor;
            set
            {
                _itemColor = value;
                OuterLabelFrame.BackgroundColor = _itemColor.Bg.AsXamarin();
                TextBoxColor.TextColor = _itemColor.Fg.AsXamarin();
                TextBoxColor.Text = FieldName;
            }
        }

        public FormsFgBgColorPicker()
        {
            InitializeComponent();
        }

        private void ChangeBackgroundColor(object sender, EventArgs e)
        {
            var colorDlg = new ColorDialog(ItemColor.Bg.AsXamarin(), FieldName + " background", NavigationManager, color =>
            {
                ItemColor.Bg = color.ToPortable();
                TextBoxColor.TextColor = _itemColor.Fg.AsXamarin();
                OuterLabelFrame.BackgroundColor = _itemColor.Bg.AsXamarin();
                TextBoxColor.Text = FieldName;
            });
            NavigationManager.PushPageOn(colorDlg);
        }
        private void ChangeForegroundColor(object sender, EventArgs e)
        {
            var colorDlg = new ColorDialog(ItemColor.Fg.AsXamarin(), FieldName + " text", NavigationManager, color =>
            {
                ItemColor.Fg = color.ToPortable();
                TextBoxColor.TextColor = _itemColor.Fg.AsXamarin();
                OuterLabelFrame.BackgroundColor = _itemColor.Bg.AsXamarin();
                TextBoxColor.Text = FieldName;
            });
            NavigationManager.PushPageOn(colorDlg);
        }

    }

}