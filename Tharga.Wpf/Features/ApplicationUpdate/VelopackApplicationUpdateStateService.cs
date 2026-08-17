using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;
using Velopack;

namespace Tharga.Wpf.ApplicationUpdate;

internal class VelopackApplicationUpdateStateService : ApplicationUpdateStateServiceBase
{
    // Velopack's own default. Set explicitly so the manager's fallback decision and the message shown to the user are guaranteed to agree.
    private const int MaximumDeltasBeforeFallback = 10;

    private readonly ILogger<VelopackApplicationUpdateStateService> _logger;

    public VelopackApplicationUpdateStateService(IConfiguration configuration, ILoggerFactory loggerFactory, ILicenseClient licenseClient, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
        _logger = loggerFactory.CreateLogger<VelopackApplicationUpdateStateService>();
    }

    protected override async Task UpdateAsync(string clientLocation)
    {
        _logger.LogInformation("Velopack UpdateAsync start. clientLocation={ClientLocation}", clientLocation);

        var mgr = new UpdateManager(clientLocation, new UpdateOptions { MaximumDeltasBeforeFallback = MaximumDeltasBeforeFallback });
        if (!mgr.IsInstalled)
        {
            var message = $"{_options.ApplicationShortName} is not installed.";
            _logger.LogInformation("Velopack UpdateAsync: {Message}", message);
            OnUpdateInfoEvent(this, message);
            return;
        }

        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            _logger.LogInformation("Velopack UpdateAsync: already up to date.");
            OnUpdateInfoEvent(this, "Already up to date.");
            // Make sure no progress bar is shown when there is nothing to download.
            _splash?.HideProgress();
            return;
        }

        var msg = VelopackUpdateMessage.Build(
            $"{newVersion.TargetFullRelease.Version}",
            newVersion.DeltasToTarget.Length,
            newVersion.DeltasToTarget.Sum(x => (long)x.Size),
            (long)newVersion.TargetFullRelease.Size,
            MaximumDeltasBeforeFallback);

        _logger.LogInformation("Velopack UpdateAsync: update available - {Msg}.", msg);

        // Now we know there is a real download about to happen — show progress bar.
        _splash?.ShowProgress();
        if (!_persistentCloseButton)
        {
            // During an actual update, hide the close button so the user can't kill it
            // mid-download. If the splash was opened from the About menu (persistent),
            // leave the close button visible per the user's intent.
            _splash?.HideCloseButton();
        }

        OnUpdateInfoEvent(this, $"Downloading {msg}.");
        try
        {
            await mgr.DownloadUpdatesAsync(newVersion, progress => _splash?.SetProgress(progress));

            OnUpdateInfoEvent(this, "Installing.");
            await BeforeRestartAsync();
            mgr.ApplyUpdatesAndRestart(newVersion);
        }
        finally
        {
            _splash?.HideProgress();
        }
    }
}