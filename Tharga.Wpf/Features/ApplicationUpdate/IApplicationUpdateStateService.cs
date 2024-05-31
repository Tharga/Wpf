namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    void ShowSplash(bool checkForUpdates = false);
    Task CheckForUpdateAsync();
}