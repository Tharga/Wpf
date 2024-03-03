using System.Windows;

namespace Tharga.Wpf.Features.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    Task StartUpdateLoop();
    void AttachMainWindow(Window mainWindow);
}