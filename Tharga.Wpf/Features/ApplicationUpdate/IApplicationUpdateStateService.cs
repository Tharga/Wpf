namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Manages application update state, splash screen display, and license checking.
/// </summary>
public interface IApplicationUpdateStateService
{
    /// <summary>Raised when update information is available.</summary>
    event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;

    /// <summary>Raised when license validation information is available.</summary>
    event EventHandler<LicenseInfoEventArgs> LicenseInfoEvent;

    /// <summary>Raised when the splash screen has completed.</summary>
    event EventHandler<SplashCompleteEventArgs> SplashCompleteEvent;

    /// <summary>
    /// Shows the splash screen, optionally checking for updates and license validity.
    /// </summary>
    /// <param name="checkForUpdates">If <c>true</c>, checks for application updates during splash.</param>
    /// <param name="showCloseButton">If <c>true</c>, displays a close button on the splash screen.</param>
    /// <param name="checkForLicense">If <c>true</c>, checks license validity during splash.</param>
    Task ShowSplashAsync(bool checkForUpdates = false, bool showCloseButton = false, bool checkForLicense = false);

    /// <summary>
    /// Checks for available application updates.
    /// </summary>
    /// <param name="source">An optional source identifier for the check.</param>
    Task CheckForUpdateAsync(string source = null);

    /// <summary>
    /// Checks whether the application has a valid license.
    /// </summary>
    /// <param name="source">An optional source identifier for the check.</param>
    /// <returns><c>true</c> if the license is valid; otherwise, <c>false</c>.</returns>
    Task<bool> CheckForLicenseAsync(string source = null);
}