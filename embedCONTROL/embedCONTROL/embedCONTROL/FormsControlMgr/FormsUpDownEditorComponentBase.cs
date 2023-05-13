using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public abstract class FormsUpDownEditorComponentBase<T> : BaseUpDownIntEditorComponent<T>
    {
        private Button _back;
        private Button _fwd;
        private Label _text;
        private Grid _grid;

        public FormsUpDownEditorComponentBase(MenuItem item, IRemoteController remote, ComponentSettings settings)
            : base(remote, settings, item)
        {
        }

        public View CreateComponent()
        {
            _grid = new Grid
            {
                BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
                ColumnSpacing = 5,
                RowSpacing = 5,
            };

            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _back = new Button
            {
                Text = "<",
                BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                HorizontalOptions = LayoutOptions.Center
            };

            _fwd = new Button
            {
                Text = ">",
                BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                HorizontalOptions = LayoutOptions.Center
            };

            _text = new Label
            {
                Text = _item.Name,
                TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
                FontSize = DrawingSettings.FontSize,
                HorizontalTextAlignment = XamarinFormsScreenManager.ToTextAlignment(DrawingSettings.Justification)
            };

            _grid.Children.Add(_back);
            Grid.SetRow(_back, 0);
            Grid.SetColumn(_back, 0);
            _grid.Children.Add(_text);
            Grid.SetRow(_text, 0);
            Grid.SetColumn(_text, 1);
            _grid.Children.Add(_fwd);
            Grid.SetRow(_fwd, 0);
            Grid.SetColumn(_fwd, 2);

            if (_item.ReadOnly)
            {
                _fwd.IsEnabled = false;
                _back.IsEnabled = false;
            }
            else
            {
                _fwd.Clicked += FwdButton_Click;
                _back.Clicked += BackBtn_Click;
            }
            return _grid;
        }

        private void FwdButton_Click(object sender, EventArgs e)
        {
            BumpCount(1);
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            BumpCount(-1);
        }

        public override void ChangeControlSettings(RenderStatus status, string str)
        {
            _text.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
            _text.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.TEXT_FIELD).AsXamarin();
            _back.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _back.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _fwd.BackgroundColor = DrawingSettings.Colors.BackgroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _fwd.TextColor = DrawingSettings.Colors.ForegroundFor(status, ColorComponentType.BUTTON).AsXamarin();
            _text.Text = str;
        }

    }
}
