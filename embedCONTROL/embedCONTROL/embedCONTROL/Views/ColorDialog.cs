using System;
using Xamarin.Forms;

namespace embedCONTROL.Views
{
    public class ColorDialog : ContentPage
    {
        private static readonly Color[] DefaultColors = new Color[]
        {
            Color.Black, Color.White, Color.Gray, Color.DarkGray, Color.LightGray, Color.Azure, Color.Aquamarine, Color.Beige,
            Color.Purple, Color.MediumPurple, Color.BlueViolet, Color.Blue, Color.LightBlue, Color.CadetBlue, Color.CornflowerBlue, Color.AliceBlue,
            Color.Green, Color.GreenYellow, Color.LightYellow, Color.Red, Color.Orange, Color.OrangeRed, Color.LightCoral, Color.Bisque
        };

        public Color CurrentColor { get; set; }
        private readonly Frame _backgroundFrame;
        private readonly Action<Color> _colorChangeNotification;
        private readonly Slider _redSlider;
        private readonly Slider _greenSlider;
        private readonly Slider _blueSlider;
        private bool _settingValue;
        private readonly INavigationManager _navigationManager;
        private int _row;
        private readonly Grid _displayGrid;

        public ColorDialog(Color initialColor, string colorName, INavigationManager navigationManager, Action<Color> changeNotification)
        {
            BackgroundColor = Color.Transparent;
            CurrentColor = initialColor;
            _colorChangeNotification = changeNotification;
            _navigationManager = navigationManager;
            Title = "Change " + colorName + " color";

            _displayGrid = new Grid
            {
                Padding = new Thickness(10),
                ColumnSpacing = 4,
                RowSpacing = 4
            };

            for (var i = 0; i < 8; i++)
            {
                _displayGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            _backgroundFrame = new Frame
            {
                BackgroundColor = CurrentColor,
                BorderColor = Color.Black
            };

            AddToGrid(_displayGrid, _backgroundFrame, IncrementRow(1), 0, 1, 8);

            AddToGrid(_displayGrid, new Label { Text = "Default Color Selections" }, IncrementRow(1), 0, 1, 8);

            for (var col = 0; col < 8; col++)
            {
                var btn = new Button { BackgroundColor = DefaultColors[col], HeightRequest = 32 };
                btn.Clicked += OnColorSelected;
                AddToGrid(_displayGrid, btn, _row, col);

                btn = new Button { BackgroundColor = DefaultColors[col + 8], HeightRequest = 32 };
                btn.Clicked += OnColorSelected;
                AddToGrid(_displayGrid, btn, _row + 1, col);

                btn = new Button { BackgroundColor = DefaultColors[col + 16], HeightRequest = 32 };
                btn.Clicked += OnColorSelected;
                AddToGrid(_displayGrid, btn, _row + 2, col);

            }

            IncrementRow(3);

            AddToGrid(_displayGrid, new Label { Text = "Custom Color Selection" }, IncrementRow(1), 0, 1, 8);


            _redSlider = new Slider(0, 255, CurrentColor.R * 255.0);
            _greenSlider = new Slider(0, 255, CurrentColor.G * 255.0);
            _blueSlider = new Slider(0, 255, CurrentColor.B * 255.0);

            _redSlider.ValueChanged += SliderValueChanged;
            _greenSlider.ValueChanged += SliderValueChanged;
            _blueSlider.ValueChanged += SliderValueChanged;

            AddToGrid(_displayGrid, _redSlider, _row, 2, 1, 6);
            AddToGrid(_displayGrid, new Label { Text = "Red" }, IncrementRow(1), 0, 1, 2);
            AddToGrid(_displayGrid, _greenSlider, _row, 2, 1, 6);
            AddToGrid(_displayGrid, new Label { Text = "Green" }, IncrementRow(1), 0, 1, 2);
            AddToGrid(_displayGrid, _blueSlider, _row, 2, 1, 6);
            AddToGrid(_displayGrid, new Label { Text = "Blue" }, IncrementRow(1), 0, 1, 2);

            var okBtn = new Button { Text = "Change Color" };
            okBtn.Clicked += OnColorChosen;
            AddToGrid(_displayGrid, okBtn, IncrementRow(1), 0, 1, 8);

            var cancelBtn = new Button { Text = "Keep Original Color" };
            cancelBtn.Clicked += OnCancel;
            AddToGrid(_displayGrid, cancelBtn, IncrementRow(1), 0, 1, 8);

            Content = _displayGrid;
        }

        private int IncrementRow(int number)
        {

            for (var i = 0; i < number; i++)
            {
                _displayGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var oldRow = _row;
            _row += number;
            return oldRow;

        }

        private void SliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_settingValue) return;

            CurrentColor = new Color(_redSlider.Value / 255.0, _greenSlider.Value / 255.0, _blueSlider.Value / 255.0);
            _backgroundFrame.BackgroundColor = CurrentColor;
        }

        private void OnCancel(object sender, EventArgs e)
        {
            _navigationManager.PopPage();
        }

        private void OnColorChosen(object sender, EventArgs e)
        {
            _navigationManager.PopPage();
            _colorChangeNotification?.Invoke(CurrentColor);
        }

        private void OnColorSelected(object sender, EventArgs e)
        {
            CurrentColor = ((Button)sender).BackgroundColor;
            _backgroundFrame.BackgroundColor = CurrentColor;
            _settingValue = true;
            _redSlider.Value = CurrentColor.R * 255.0;
            _greenSlider.Value = CurrentColor.G * 255.0;
            _blueSlider.Value = CurrentColor.B * 255.0;
            _settingValue = false;
        }

        private void AddToGrid(Grid theGrid, View item, int row, int col, int rowSpan = 1, int colSpan = 1)
        {
            Grid.SetRow(item, row);
            Grid.SetColumn(item, col);
            Grid.SetColumnSpan(item, colSpan);
            Grid.SetRowSpan(item, rowSpan);
            theGrid.Children.Add(item);
        }
    }

}
