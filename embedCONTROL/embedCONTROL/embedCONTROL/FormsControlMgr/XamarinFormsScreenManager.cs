using System;
using embedCONTROL.ControlMgr;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using Xamarin.Forms;
using MenuItem = tcMenuControlApi.MenuItems.MenuItem;

namespace embedCONTROL.FormsControlMgr
{
    public class XamarinFormsScreenManager : IScreenManager
    {
        private readonly ScrollView _scrollView;
        private readonly IRemoteController _controller;
        private readonly int _cols;
        private int _level;
        private Grid _currentGrid;

        public int DefaultFontSize => 16;

        public XamarinFormsScreenManager(IRemoteController controller, ScrollView scrollView, int cols)
        {
            _scrollView = scrollView;
            _controller = controller;
            _cols = cols;

            Clear();
        }

        public void Clear()
        {
            _currentGrid = new Grid
            {
                ColumnSpacing = 3,
                RowSpacing = 3
            };
            for (var i = 0; i < _cols; i++)
            {
                _currentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        public void AddStaticLabel(string label, ComponentSettings settings, bool isHeader)
        {
            var lbl = new Label
            {
                HorizontalTextAlignment = ToTextAlignment(settings.Justification),
                Text = label,
                FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
                TextColor = settings.Colors.ForegroundFor(RenderStatus.Normal, ColorComponentType.TEXT_FIELD).AsXamarin(),
            };

            AddToGridInPosition(settings, lbl);
        }

        public IEditorComponent AddUpDownInteger(MenuItem item, ComponentSettings settings)
        {
            var analogEditor = new IntegerUpDownEditorComponent(item, _controller, settings);
            AddToGridInPosition(settings, analogEditor.CreateComponent());
            return analogEditor;
        }

        public IEditorComponent AddUpDownScroll(MenuItem item, ComponentSettings settings)
        {
            var scrollEditor = new ScrollUpDownEditorComponent(item, _controller, settings);
            AddToGridInPosition(settings, scrollEditor.CreateComponent());
            return scrollEditor;
        }

        public IEditorComponent AddBooleanButton(MenuItem item, ComponentSettings settings)
        {
            var boolBtn = new BoolButtonEditorComponent(item, _controller, settings);
            AddToGridInPosition(settings, boolBtn.CreateComponent());
            return boolBtn;
        }

        public IEditorComponent AddRgbColorControl(MenuItem item, ComponentSettings settings)
        {
            var colorRgb = new RgbColorEditorComponent(item, _controller, settings);
            AddToGridInPosition(settings, colorRgb.CreateComponent());
            return colorRgb;
        }

        public IEditorComponent AddDateEditorComponent(MenuItem item, ComponentSettings settings)
        {
            if (item is EditableTextMenuItem textFld && textFld.EditType == EditItemType.GREGORIAN_DATE)
            {
                var dateEditor = new DateFieldEditorComponent(_controller, settings, item);
                AddToGridInPosition(settings, dateEditor.CreateComponent());
                return dateEditor;
            }
            else
            {
                throw new ArgumentException($"{item} was not of gregorian date type");
            }
        }

        public IEditorComponent AddTimeEditorComponent(MenuItem item, ComponentSettings settings)
        {
            if (item is EditableTextMenuItem textFld && (textFld.EditType == EditItemType.TIME_12H || textFld.EditType == EditItemType.TIME_24H || textFld.EditType == EditItemType.TIME_24_HUNDREDS))
            {
                var dateEditor = new TimeFieldEditorComponent(_controller, settings, item);
                AddToGridInPosition(settings, dateEditor.CreateComponent());
                return dateEditor;
            }
            else
            {
                throw new ArgumentException($"{item} was not of gregorian date type");
            }
        }

        public IEditorComponent AddHorizontalSlider(MenuItem item, ComponentSettings settings)
        {
            var slider = new HorizontalSliderAnalogComponent(_controller, settings, item, _controller.ManagedMenu);
            AddToGridInPosition(settings, slider.CreateComponent());
            return slider;
        }

        public IEditorComponent AddTextEditor<TVal>(MenuItem item, ComponentSettings settings)
        {
            var textEd = new TextFieldEditorComponent<TVal>(_controller, settings, item);
            AddToGridInPosition(settings, textEd.CreateComponent());
            return textEd;
        }

        public IEditorComponent AddListEditor(MenuItem item, ComponentSettings settings, RuntimeListStringAdapter adapter)
        {
            var listEd = new ListEditorComponent(_controller, settings, item, adapter);
            AddToGridInPosition(settings, listEd.CreateComponent());
            return listEd;
        }

        private void AddToGridInPosition(ComponentSettings settings, View sp)
        {
            _currentGrid.Children.Add(sp);
            SetGridPositioning(settings, sp);
            _currentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        public void EndNesting()
        {
            if (--_level == 0)
            {
                _scrollView.Content = _currentGrid;
            }
        }

        public void StartNesting()
        {
            _level++;
        }

        private static void SetGridPositioning(ComponentSettings settings, View lbl)
        {
            Grid.SetRow(lbl, settings.Position.Row);
            Grid.SetColumn(lbl, settings.Position.Col);
            Grid.SetRowSpan(lbl, settings.Position.RowSpan);
            Grid.SetColumnSpan(lbl, settings.Position.ColSpan);
        }

        public static TextAlignment ToTextAlignment(PortableAlignment justification)
        {
            switch (justification)
            {
                case PortableAlignment.Right:
                    return TextAlignment.End;
                case PortableAlignment.Center:
                    return TextAlignment.Center;
                case PortableAlignment.Left:
                default:
                    return TextAlignment.Start;
            }
        }
    }
}
