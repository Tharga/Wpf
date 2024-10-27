namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    event EventHandler<SplashClosedEventArgs> SplashClosedEvent;
    void ShowSplash(bool checkForUpdates = false, bool showCloseButton = false);
}