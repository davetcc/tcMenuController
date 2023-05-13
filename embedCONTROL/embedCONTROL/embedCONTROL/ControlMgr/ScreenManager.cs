using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using tcMenuControlApi.Commands;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.Protocol;

namespace embedCONTROL.ControlMgr
{
    public enum PortableAlignment
    {
        Left,
        Right,
        Center
    }

    public enum RenderStatus { Normal, RecentlyUpdated, EditInProgress, CorrelationError }

    [Flags]
    public enum RedrawingMode { 
        ShowName = 1, 
        ShowValue = 2, 
        ShowNameInLabel = 4, 
        ShowNameAndValue = ShowName | ShowValue,
        ShowLabelNameAndValue = ShowNameInLabel | ShowValue
    }

    public class ComponentPositioning
    {
        public int Row { get; }
        public int Col { get; }
        public int RowSpan { get; }
        public int ColSpan { get; }

        public ComponentPositioning(int row, int col, int rowSpan = 1, int colSpan = 1)
        {
            Row = row;
            Col = col;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }

    public interface IEditorComponent
    {
        void OnItemUpdated(AnyMenuState newValue);

        void OnCorrelation(CorrelationId correlationId, AckStatus status);

        void Tick();

        void FromStore(XElement element);

        XElement ToStore();
    }

    public class ComponentSettings
    {
        public static readonly ComponentSettings NO_COMPONENT = new ComponentSettings(new NullConditionalColoring(), 0, PortableAlignment.Left, null);

        public int FontSize { get; }
        public IConditionalColoring Colors { get; }
        public PortableAlignment Justification { get; }
        public ComponentPositioning Position { get; }
        public RedrawingMode DrawMode { get; }
        public bool Customised { get; }

        public ComponentSettings(IConditionalColoring colors, int fontSize, PortableAlignment justification,
            ComponentPositioning position, RedrawingMode mode = RedrawingMode.ShowValue, bool custom = false)
        {
            FontSize = fontSize;
            Colors = colors;
            Justification = justification;
            Position = position;
            DrawMode = mode;
            Customised = custom;
        }
    }

    public interface IScreenManager
    {
        int DefaultFontSize { get; }
        void Clear();
        void StartNesting();
        void EndNesting();
        void AddStaticLabel(string label, ComponentSettings position, bool isHeader);
        IEditorComponent AddUpDownInteger(MenuItem item, ComponentSettings settings);
        IEditorComponent AddUpDownScroll(MenuItem item, ComponentSettings settings);
        IEditorComponent AddBooleanButton(MenuItem item, ComponentSettings settings);
        IEditorComponent AddTextEditor<TVal>(MenuItem enumItem, ComponentSettings settings);
        IEditorComponent AddListEditor(MenuItem item, ComponentSettings settings, RuntimeListStringAdapter adapter);
        IEditorComponent AddDateEditorComponent(MenuItem item, ComponentSettings settings);
        IEditorComponent AddTimeEditorComponent(MenuItem item, ComponentSettings settings);
        IEditorComponent AddHorizontalSlider(MenuItem item, ComponentSettings settings);
        IEditorComponent AddRgbColorControl(MenuItem item, ComponentSettings settings);

    }

}