namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    event EventHandler<EventArgs> SplashClosedEvent;
    void ShowSplash(bool checkForUpdates = false, bool autoClose = true);
    Task CheckForUpdateAsync(string source = null);
}