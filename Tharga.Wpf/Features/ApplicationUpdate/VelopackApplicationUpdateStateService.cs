using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;
using Velopack;

namespace Tharga.Wpf.ApplicationUpdate;

internal class VelopackApplicationUpdateStateService : ApplicationUpdateStateServiceBase
{
    public VelopackApplicationUpdateStateService(IConfiguration configuration, ILoggerFactory loggerFactory, ILicenseClient licenseClient, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
    }

    protected override async Task UpdateAsync(string clientLocation)
    {
        var mgr = new UpdateManager(clientLocation);
        if (!mgr.IsInstalled)
        {
            var message = $"{_options.ApplicationShortName} is not installed.";
            OnUpdateInfoEvent(this, message);
            return;
        }

        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            OnUpdateInfoEvent(this, "No new version found.");
            return;
        }

        OnUpdateInfoEvent(this, "Downloading...");
        await mgr.DownloadUpdatesAsync(newVersion);

        OnUpdateInfoEvent(this, "Installing...");
        mgr.ApplyUpdatesAndRestart(newVersion);
    }
}