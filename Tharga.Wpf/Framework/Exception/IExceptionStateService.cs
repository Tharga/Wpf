using System.Windows;

namespace Tharga.Wpf.Framework.Exception;

public interface IExceptionStateService
{
    void AttachMainWindow(Window mainWindow);
    void FallbackHandler(System.Exception exception);
}