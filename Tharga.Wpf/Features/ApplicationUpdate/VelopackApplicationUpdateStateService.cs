using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal class VelopackApplicationUpdateStateService : ApplicationUpdateStateServiceBase
{
    public VelopackApplicationUpdateStateService(IConfiguration configuration, ILoggerFactory loggerFactory, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
    }

    //protected override void Start()
    //{
    //    //try
    //    //{
    //    //    var f = string.Empty;
    //    //    if (firstRun) f = " (firstRun)";

    //    //    if (_options.Debug) MessageBox.Show($"Every Run {f}", nameof(OnEveryRun), MessageBoxButton.OK, MessageBoxImage.Information);

    //    //    AddLogString($"--- Start {nameof(OnEveryRun)} {f} ---");

    //    //    tools.SetProcessAppUserModelId();
    //    //    _ = ShowSplashWithRetryAsync(firstRun);
    //    //    _ = StartUpdateLoop();
    //    //}
    //    //catch (Exception e)
    //    //{
    //    //    AddLogString($"Error: {e.Message} @{e.StackTrace}");
    //    //    _logger.LogError(e, e.Message);
    //    //    Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
    //    //    MessageBox.Show(e.Message, "Every Run", MessageBoxButton.OK, MessageBoxImage.Error);
    //    //}
    //    //finally
    //    //{
    //    //    AddLogString($"--- End {nameof(OnEveryRun)} ---");
    //    //}
    //}

    //public static async Task UpdateMyApp()
    //{
    //    var path = @"C:\dev\Eplicta\Conserver\Releases";
    //    //"https://location.blob.core.windows.net/container"

    //    var mgr = new UpdateManager(path);
    //    MessageBox.Show($"Current version is {mgr.AppId} {mgr.CurrentVersion}. IsInstalled={mgr.IsInstalled}. IsPortable={mgr.IsPortable}. Update Pending Restart is {mgr.UpdatePendingRestart}.");

    //    var newVersion = await mgr.CheckForUpdatesAsync();
    //    if (newVersion == null)
    //    {
    //        MessageBox.Show("No new version found.");
    //        return;
    //    }

    //    MessageBox.Show("Downloading update.");
    //    await mgr.DownloadUpdatesAsync(newVersion);
    //    MessageBox.Show("Apply update and restart when done.");
    //    mgr.ApplyUpdatesAndRestart(newVersion);
    //}

    protected override Task UpdateAsync(string clientLocation)
    {
        throw new NotImplementedException();
    }
}