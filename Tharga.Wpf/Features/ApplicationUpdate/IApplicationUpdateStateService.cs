namespace Tharga.Wpf.Features.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    void ShowSplash();
    //Task UpdateClientApplication();
    //Task StartUpdateLoop();
    //void AttachMainWindow(Window mainWindow);
}