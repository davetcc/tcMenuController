using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;

namespace embedCONTROL.FormsControlMgr
{
    public abstract class FormsEditorBase<T> : BaseTextEditorComponent<T>
    {
        protected View MakeTextComponent(View entryField, bool needSendBtn, EventHandler sendHandler = null)
        {
            var needLabel = (DrawingSettings.DrawMode & RedrawingMode.ShowNameInLabel) != 0;

            if (!needLabel && !needSendBtn) return entryField;

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            var col = 0;
            if (needLabel)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(entryField, col++);
                var lbl = new Label
                {
                    Text = _item.Name,
                    TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
                    FontSize = DrawingSettings.FontSize,
                    Padding = new Thickness(2, 5)
                };
                grid.Children.Add(lbl);

            }
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Grid.SetColumn(entryField, col++);
            grid.Children.Add(entryField);

            if (needSendBtn)
            {
                var sendButton = new Button
                {
                    Text = "Send",
                    BackgroundColor = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                    TextColor = DrawingSettings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.BUTTON).AsXamarin(),
                    HorizontalOptions = LayoutOptions.Center
                };

                if (sendHandler != null) sendButton.Clicked += sendHandler;

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Grid.SetColumn(sendButton, col);
                grid.Children.Add(sendButton);
            }

            return grid;
        }

        protected FormsEditorBase(IRemoteController controller, ComponentSettings settings, tcMenuControlApi.MenuItems.MenuItem item) 
            : base(controller, settings, item)
        {
        }
    }
}
