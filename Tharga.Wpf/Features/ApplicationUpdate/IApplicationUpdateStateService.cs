namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationUpdateStateService
{
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    event EventHandler<LicenseInfoEventArgs> LicenseInfoEvent;
    event EventHandler<SplashCompleteEventArgs> SplashCompleteEvent;
    Task ShowSplashAsync(bool checkForUpdates = false, bool showCloseButton = false, bool checkForLicense = false);
    Task CheckForUpdateAsync(string source = null);
    Task<bool> CheckForLicenseAsync(string source = null);
}