using System.Windows;

namespace Tharga.Wpf.ApplicationUpdate;

public interface ISplash
{
    void UpdateInfo(string message);
    void Show();
    void SetErrorMessage(string message);
    void ShowCloseButton();
    void Close();
    void SetOwner(Window mainWindow);
}