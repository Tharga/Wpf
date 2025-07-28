using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Squirrel;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal class SquirrelApplicationUpdateStateService : ApplicationUpdateStateServiceBase
{
    private readonly ILogger<SquirrelApplicationUpdateStateService> _logger;

    public SquirrelApplicationUpdateStateService(IConfiguration configuration, ILoggerFactory loggerFactory, ILicenseClient licenseClient, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
        ExeLocation = SquirrelRuntimeInfo.EntryExePath;
        _logger = loggerFactory.CreateLogger<SquirrelApplicationUpdateStateService>();

        SquirrelAwareApp.HandleEvents(OnInitialInstall, OnAppInstall, OnAppObsoleted, OnAppUninstall, OnEveryRun);
    }

    protected override string ExeLocation { get; }

    protected override async Task UpdateAsync(string clientLocation)
    {
        using var mgr = new UpdateManager(clientLocation);
        if (!mgr.IsInstalledApp)
        {
            var message = $"{_options.ApplicationShortName} is not installed.";
            OnUpdateInfoEvent(this, message);
            return;
        }

        var updateInfo = await mgr.CheckForUpdate();
        if (updateInfo.CurrentlyInstalledVersion.Version == updateInfo.FutureReleaseEntry.Version)
        {
            OnUpdateInfoEvent(this, "Already up to date.");
            return;
        }

        await ShowSplashWithRetryAsync(false);

        OnUpdateInfoEvent(this, $"Updating to latest version, {updateInfo.FutureReleaseEntry.Version}.");

        var newVersion = await mgr.UpdateApp();
        if (newVersion != null)
        {
            await _tabNavigationStateService.CloseAllTabsAsync(true);

            OnUpdateInfoEvent(this, "Restarting.");
            UpdateManager.RestartApp();
        }
    }

    //NOTE: Initial Install
    private void OnInitialInstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            if (_options.Debug) MessageBox.Show("Initial Install", nameof(OnInitialInstall), MessageBoxButton.OK, MessageBoxImage.Information);

            AddLogString($"--- Start {nameof(OnInitialInstall)} ---");

            var name = GetShortcutName();

            CreateShortcut();

            _ = ShowSplashWithRetryAsync(false, $"Installing {name}.");
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            MessageBox.Show(e.Message, "Initial Install", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            AddLogString($"--- End {nameof(OnInitialInstall)} ---");
        }
    }

    //NOTE: Updated to new version
    private void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            if (_options.Debug) MessageBox.Show("App Install", nameof(OnAppInstall), MessageBoxButton.OK, MessageBoxImage.Information);

            AddLogString($"--- Start {nameof(OnAppInstall)} ---");

            //var name = GetShortcutName();
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            MessageBox.Show(e.Message, "App Install", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            AddLogString($"--- End {nameof(OnAppInstall)} ---");
        }
    }

    //NOTE: Called when the app is no longer the latest version (A new version is installed)
    private void OnAppObsoleted(SemanticVersion version, IAppTools tools)
    {
        try
        {
            if (_options.Debug) MessageBox.Show("App Obsoleted", nameof(OnAppObsoleted), MessageBoxButton.OK, MessageBoxImage.Information);

            AddLogString($"--- Start {nameof(OnAppObsoleted)} ---");

            //var name = GetShortcutName();
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            MessageBox.Show(e.Message, "App Obsoleted", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            AddLogString($"--- End {nameof(OnAppObsoleted)} ---");
        }
    }

    //NOTE: Called when the app in uninstalled
    private void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            if (_options.Debug) MessageBox.Show("App Uninstall", nameof(OnAppUninstall), MessageBoxButton.OK, MessageBoxImage.Information);

            AddLogString($"--- Start {nameof(OnAppUninstall)} ---");

            var name = GetShortcutName();

            ShortcutHelper.RemoveShortcut(name);

            _ = ShowSplashWithRetryAsync(false, $"Uninstalling {name}.");
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            MessageBox.Show(e.Message, "App Uninstall", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            AddLogString($"--- End {nameof(OnAppUninstall)} ---");
        }
    }

    //NOTE: Starts on every run
    private void OnEveryRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        try
        {
            var f = string.Empty;
            if (firstRun) f = " (firstRun)";

            if (_options.Debug) MessageBox.Show($"Every Run {f}", nameof(OnEveryRun), MessageBoxButton.OK, MessageBoxImage.Information);

            AddLogString($"--- Start {nameof(OnEveryRun)} {f} ---");

            tools.SetProcessAppUserModelId();
            _ = ShowSplashWithRetryAsync(firstRun);
            //_ = StartUpdateLoop();
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            MessageBox.Show(e.Message, "Every Run", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            AddLogString($"--- End {nameof(OnEveryRun)} ---");
        }
    }

    private void CreateShortcut()
    {
        var entryExePath = SquirrelRuntimeInfo.EntryExePath;
        var pos = entryExePath.LastIndexOf("\\", StringComparison.Ordinal);
        var exeName = entryExePath.Substring(pos + 1);
        AddLogString($"Exe name is '{exeName}'.");

        var baseDirectory = GetDirectory();
        var path = Path.Combine(baseDirectory, exeName);
        AddLogString($"Path is '{path}'.");

        var iconPath = Path.Combine(baseDirectory, "app.ico");
        AddLogString($"IconPath is '{iconPath}'.");

        var iconInfo = new ShortcutHelper.IconInfo { Path = iconPath };
        var name = GetShortcutName();
        var description = _options.ApplicationFullName ?? $"{_options.CompanyName} {_options.ApplicationShortName}".Trim();
        AddLogString($"Description is '{description}'.");

        ShortcutHelper.CreateShortcut(path, name, description, iconInfo);
        AddLogString($"Shortcut created with name '{name}'.");
    }

    private static string GetDirectory()
    {
        var baseDirectory = SquirrelRuntimeInfo.BaseDirectory;
        var pos = baseDirectory.TrimEnd('\\').LastIndexOf("\\", StringComparison.Ordinal);
        baseDirectory = baseDirectory.Substring(0, pos);
        return baseDirectory;
    }

    private string GetShortcutName()
    {
        var name = _options.ApplicationShortName;
        var response = _environmentName == "Production" ? name : $"{name} {_environmentName}";
        return response;
    }
}