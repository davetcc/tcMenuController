using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using embedCONTROL.ControlMgr;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class HorizontalSliderAnalogComponent : FormsEditorBase<int>
    {
        private SKCanvasView _canvas;
        private RenderStatus _lastStatus = RenderStatus.Normal;
        private string _lastStr = "";
        private readonly MenuTree _tree;
        private int _startingValue;
        private int _displayWidth;
        private double _scale;

        public HorizontalSliderAnalogComponent(IRemoteController controller, ComponentSettings settings, MenuItem item, MenuTree tree)
            : base(controller, settings, item)
        {
            _tree = tree;
        }

        public View CreateComponent()
        {
            _canvas = new SKCanvasView
            {
                HeightRequest = 40
            };
            _canvas.PaintSurface += OnPaintSurface;
            
            if (IsItemEditable(_item))
            {
                var panRecognizer = new PanGestureRecognizer();
                panRecognizer.PanUpdated += OnPanAdjusted;
                _canvas.GestureRecognizers.Add(panRecognizer);
            }

            return MakeTextComponent(_canvas, false);
        }

        private async void OnPanAdjusted(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Completed)
            {
                await SendItemAbsolute();
            }

            if (!(_tree.GetState(_item) is MenuState<int> intState) || !(_item is AnalogMenuItem analog)) return;

            if (e.StatusType == GestureStatus.Started)
            {
                _startingValue = intState.Value;
            }
            else
            {
                var oneTick = (_displayWidth / _scale) / analog.MaximumValue;
                var value = Math.Max(0, Math.Min(analog.MaximumValue, _startingValue + (int)(e.TotalX / oneTick)));
                _tree.ChangeItemState(_item, new MenuState<int>(_item, true, intState.Active, value));
                OnItemUpdated(intState);
            }

            _canvas.InvalidateSurface();
            logger.Information($"{e.TotalX}, {e.TotalY}");

        }

        private async Task SendItemAbsolute()
        {
            if (_status == RenderStatus.EditInProgress) return;
            try
            {
                if (_tree.GetState(_item) is MenuState<int> intState)
                {
                    var correlation = await _remoteController.SendAbsoluteChange(_item, intState.Value);
                    EditStarted(correlation);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to send message to {_remoteController.Connector.ConnectionName}");
            }
        }

        protected void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            _displayWidth = info.Width;
            _scale = e.Info.Height / ((SKCanvasView) sender).Height;

            if (!(_tree.GetState(_item) is MenuState<int> intState) || !(_item is AnalogMenuItem analog)) return;
            
            var currentPercentage = intState.Value / (float)analog.MaximumValue;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = DrawingSettings.Colors.BackgroundFor(RenderStatus.Normal, ColorComponentType.HIGHLIGHT).AsSkia(),
                StrokeWidth = 1
            };
            canvas.DrawRect(new SKRect(0, 0, info.Width * currentPercentage, info.Height), paint);

            paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = DrawingSettings.Colors.BackgroundFor(_lastStatus, ColorComponentType.TEXT_FIELD).AsSkia(),
                StrokeWidth = 2
            };
            canvas.DrawRect(new SKRect(info.Width * currentPercentage, 0, info.Width, info.Height), paint);

            paint = new SKPaint
            {
                Color = DrawingSettings.Colors.ForegroundFor(_lastStatus, ColorComponentType.TEXT_FIELD).AsSkia(),
                TextSize = (float) (DrawingSettings.FontSize * _scale),
                Typeface = SKTypeface.FromFamilyName("Arial"),
            };

            var text = _lastStr;// + " " + currentPercentage;

            SKRect bounds = new SKRect();
            float textWidth = paint.MeasureText(text, ref bounds);
            canvas.DrawText(text, (info.Width - textWidth) / 2.0F, info.Height / 2.0F - bounds.MidY, paint);
        }

        public override void ChangeControlSettings(RenderStatus status, string text)
        {
            _lastStatus = status;
            _lastStr = text;

            _canvas.InvalidateSurface();
        }
    }
}
