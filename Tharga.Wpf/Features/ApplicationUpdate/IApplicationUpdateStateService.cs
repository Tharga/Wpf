namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    event EventHandler<SplashCompleteEventArgs> SplashCompleteEvent;
    Task ShowSplashAsync(bool checkForUpdates = false, bool showCloseButton = false);
    Task CheckForUpdateAsync(string source = null);
}