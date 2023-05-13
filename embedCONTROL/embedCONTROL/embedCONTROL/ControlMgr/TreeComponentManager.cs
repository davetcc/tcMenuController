using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using embedCONTROL.FormsControlMgr;
using embedCONTROL.Models;
using embedCONTROL.Services;
using embedCONTROL.ViewModels;
using tcMenuControlApi.Commands;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteStates;

namespace embedCONTROL.ControlMgr
{
    public sealed class TreeComponentManager : IDisposable
    {
        private readonly Dictionary<int, IEditorComponent> _editorComponents = new Dictionary<int, IEditorComponent>();
        private readonly Timer _timer = new Timer(100);
        private readonly TcMenuConnection _connection;
        private readonly PrefsAppSettings _appSettings;
        private readonly IScreenManager _screenManager;
        private readonly IDialogViewer _dialogViewer;
        private readonly int _cols;
        int _currRow = 0;
        int _currCol = 0;

        public TreeComponentManager(IScreenManager screenManager, TcMenuConnection connection,
            PrefsAppSettings appSettings, IDialogViewer dialogViewer)
        {
            _appSettings = appSettings;
            _cols = appSettings.DefaultNumColumms;
            _screenManager = screenManager;
            _connection = connection;
            _dialogViewer = dialogViewer;

            _timer.Elapsed += Timer_Triggered;
            _timer.Start();

            _connection.Controller.MenuChangedEvent += Controller_MenuChangedEvent;
            _connection.Controller.AcknowledgementsReceived += Controller_AcknowledgementsReceived;
            _connection.Controller.DialogUpdatedEvent += Controller_DialogUpdatedEvent;
            _connection.Controller.Connector.ConnectionChanged += Connector_ConnectionChanged;

            // handle the case where it's already connected really quick!
            if (_connection.Controller.Connector.AuthStatus == AuthenticationStatus.CONNECTION_READY)
            {
                Connector_ConnectionChanged(AuthenticationStatus.CONNECTION_READY);
            }
        }

        private void Connector_ConnectionChanged(AuthenticationStatus status)
        {
            if (status == AuthenticationStatus.CONNECTION_READY)
            {
                ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
                {
                    _currRow = 0;
                    _currCol = 0;
                    _screenManager.Clear();
                    _editorComponents.Clear();
                    RenderMenuRecursive(MenuTree.ROOT, _screenManager, _appSettings);
                });
            }
        }

        private void RenderMenuRecursive(SubMenuItem sub, IScreenManager screenManager, PrefsAppSettings appSettings)
        {
            var tree = _connection.Controller.ManagedMenu;
            var prefsColoring = new PrefsConditionalColoring(appSettings);

            var menuName = sub == MenuTree.ROOT ? _connection.Name : sub.Name;
            screenManager.AddStaticLabel(menuName, new ComponentSettings(prefsColoring,
                screenManager.DefaultFontSize,
                PortableAlignment.Left, nextRowCol(true)), true);

            screenManager.StartNesting();
            foreach (var item in tree.GetMenuItems(sub))
            {
                if (!item.Visible) continue;
                switch (item)
                {
                    case SubMenuItem s:
                        RenderMenuRecursive(s, screenManager, appSettings);
                        break;
                    case BooleanMenuItem boolItem:
                        _editorComponents.Add(boolItem.Id, screenManager.AddBooleanButton(boolItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowNameAndValue)));
                        break;
                    case ActionMenuItem actionItem:
                        _editorComponents.Add(actionItem.Id, screenManager.AddBooleanButton(actionItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowName)));
                        break;
                    case AnalogMenuItem analogItem:
                        _editorComponents.Add(analogItem.Id, screenManager.AddHorizontalSlider(analogItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowNameAndValue)));
                        break;
                    case Rgb32MenuItem rgb:
                        _editorComponents.Add(rgb.Id, screenManager.AddRgbColorControl(rgb, 
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowLabelNameAndValue)));
                        break;
                    case EnumMenuItem enumItem:
                        _editorComponents.Add(enumItem.Id, screenManager.AddUpDownInteger(enumItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowNameAndValue)));
                        break;
                    case ScrollChoiceMenuItem scrollItem:
                        _editorComponents.Add(scrollItem.Id, screenManager.AddUpDownScroll(scrollItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowNameAndValue)));
                        break;
                    case FloatMenuItem floatItem:
                        _editorComponents.Add(floatItem.Id, screenManager.AddTextEditor<float>(floatItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowNameAndValue)));
                        break;
                    case RuntimeListMenuItem listItem:
                        RuntimeListStringAdapter adapter = null;
                        if (listItem.Name == "Remote") adapter = DefaultListStringAdapters.RemoteFormattingAdapter;
                        screenManager.AddStaticLabel(item.Name, new ComponentSettings(prefsColoring,
                            screenManager.DefaultFontSize,
                            PortableAlignment.Left, nextRowCol(true)), false);
                        _editorComponents.Add(listItem.Id, screenManager.AddListEditor(listItem,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Left, nextRowCol(true)),
                            adapter));
                        break;
                    case EditableTextMenuItem textItem:
                        if (textItem.EditType == EditItemType.GREGORIAN_DATE)
                        {
                            _editorComponents.Add(textItem.Id, screenManager.AddDateEditorComponent(textItem,
                                new ComponentSettings(prefsColoring,
                                    screenManager.DefaultFontSize,
                                    PortableAlignment.Center, nextRowCol(),
                                    RedrawingMode.ShowLabelNameAndValue)));
                        }
                        else if (textItem.EditType == EditItemType.TIME_24_HUNDREDS ||
                                 textItem.EditType == EditItemType.TIME_12H ||
                                 textItem.EditType == EditItemType.TIME_24H)
                        {
                            _editorComponents.Add(textItem.Id, screenManager.AddTimeEditorComponent(textItem,
                                new ComponentSettings(prefsColoring,
                                    screenManager.DefaultFontSize,
                                    PortableAlignment.Center, nextRowCol(),
                                    RedrawingMode.ShowLabelNameAndValue)));
                        }
                        else
                        {
                            _editorComponents.Add(textItem.Id, screenManager.AddTextEditor<string>(textItem,
                                new ComponentSettings(prefsColoring,
                                    screenManager.DefaultFontSize,
                                    PortableAlignment.Center, nextRowCol(),
                                    RedrawingMode.ShowLabelNameAndValue)));
                        }

                        break;
                    case LargeNumberMenuItem largeNum:
                        _editorComponents.Add(largeNum.Id, screenManager.AddTextEditor<decimal>(largeNum,
                            new ComponentSettings(prefsColoring,
                                screenManager.DefaultFontSize,
                                PortableAlignment.Center, nextRowCol(),
                                RedrawingMode.ShowLabelNameAndValue)));
                        break;
                }

                if (_editorComponents.ContainsKey(item.Id) && tree.GetState(item) != null)
                {
                    _editorComponents[item.Id].OnItemUpdated(tree.GetState(item));
                }
            }

            screenManager.EndNesting();
        }

        private ComponentPositioning nextRowCol(bool startNewRow = false)
        {
            if (startNewRow && _currCol != 0)
            {
                _currRow++;
                _currCol = 0;
            }

            var pos = new ComponentPositioning(_currRow, _currCol, 1, startNewRow ? _cols : 1);

            if (++_currCol >= _cols || startNewRow)
            {
                _currRow++;
                _currCol = 0;
            }

            return pos;
        }

        private void Timer_Triggered(object sender, ElapsedEventArgs e)
        {
            ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                foreach (var component in _editorComponents)
                {
                    component.Value.Tick();
                }
            });
        }


        private void Controller_DialogUpdatedEvent(tcMenuControlApi.Protocol.CorrelationId cor,
            tcMenuControlApi.Commands.DialogMode mode, string hdr, string msg,
            tcMenuControlApi.Commands.MenuButtonType b1, tcMenuControlApi.Commands.MenuButtonType b2)
        {
            ApplicationContext.Instance.ThreadMarshaller.OnUiThread(() =>
            {
                _dialogViewer.Show(mode == DialogMode.SHOW);
                if (mode == DialogMode.SHOW)
                {
                    _dialogViewer.SetButton1(b1);
                    _dialogViewer.SetButton2(b2);
                    _dialogViewer.SetText(hdr, msg);
                }
            });
        }

        private void Controller_AcknowledgementsReceived(tcMenuControlApi.Protocol.CorrelationId correlation,
            tcMenuControlApi.Commands.AckStatus status)
        {
            foreach (var uiItem in _editorComponents.Values)
            {
                uiItem.OnCorrelation(correlation, status);
            }
        }

        private void Controller_MenuChangedEvent(MenuItem changed, bool valueOnly)
        {
            if (_editorComponents?.ContainsKey(changed.Id) ?? false)
            {
                _editorComponents[changed.Id].OnItemUpdated(_connection.Controller.ManagedMenu.GetState(changed));
            }
        }

        public void Dispose()
        {
            var controller = _connection.Controller;
            if (controller != null)
            {
                controller.MenuChangedEvent -= Controller_MenuChangedEvent;
                controller.AcknowledgementsReceived -= Controller_AcknowledgementsReceived;
                controller.DialogUpdatedEvent -= Controller_DialogUpdatedEvent;
                controller.Connector.ConnectionChanged -= Connector_ConnectionChanged;
            }

            _timer.Stop();
        }
    }
}