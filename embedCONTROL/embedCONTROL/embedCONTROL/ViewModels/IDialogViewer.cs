using tcMenuControlApi.Commands;

namespace embedCONTROL.ViewModels
{
    public interface IDialogViewer
    {
        void SetButton1(MenuButtonType type);
        void SetButton2(MenuButtonType type);
        void Show(bool visible);
        void SetText(string title, string subject);
    }
    
}