using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Squirrel;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal class ApplicationUpdateStateService : IApplicationUpdateStateService
{
    private readonly IApplicationDownloadService _applicationDownloadService;
    private readonly ITabNavigationStateService _tabNavigationStateService;
    private readonly ThargaWpfOptions _options;
    private readonly Window _mainWindow;
    private readonly ILogger<ApplicationUpdateStateService> _logger;
    private readonly System.Timers.Timer _timer;
    private readonly string _environmentName;
    private readonly string _version;
    private readonly string _exeLocation;
    private ISplash _splash;
    private string _applicationLocation;
    private string _applicationLocationSource;

    public ApplicationUpdateStateService(IConfiguration configuration, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow, ILogger<ApplicationUpdateStateService> logger)
    {
        _applicationDownloadService = applicationDownloadService;
        _tabNavigationStateService = tabNavigationStateService;
        _options = options;
        _mainWindow = mainWindow;
        _logger = logger;
        _environmentName = configuration.GetSection("Environment").Value;

        var entryAssembly = Assembly.GetEntryAssembly();
        var assemblyName = entryAssembly?.GetName();
        var version = assemblyName?.Version?.ToString();
        _version = version == "1.0.0.0" ? null : version;
        _exeLocation = SquirrelRuntimeInfo.EntryExePath; //entryAssembly?.Location;

        var interval = options.CheckForUpdateInterval;
        if (interval != null && interval > TimeSpan.Zero)
        {
            _timer = new System.Timers.Timer { AutoReset = true, Enabled = false, Interval = interval.Value.TotalMilliseconds };
            _timer.Elapsed += async (_, _) =>
            {
                try
                {
                    await UpdateClientApplication();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            };
        }

        if (options.Debug)
        {
            Task.Run(async () =>
            {
                var result = await _applicationDownloadService.GetApplicationLocationAsync();
                _applicationLocation = result.ApplicationLocation;
                _applicationLocationSource = result.ApplicationLocationSource;
            });
        }

        if (interval != null)
        {
            SquirrelAwareApp.HandleEvents(OnInitialInstall, OnAppInstall, OnAppObsoleted, OnAppUninstall, OnEveryRun);

            _mainWindow.IsVisibleChanged += (_, _) =>
            {
                if (_mainWindow.Visibility != Visibility.Visible) _splash?.Hide();
            };
        }
    }

    public event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;

    //NOTE: Initial Install
    private void OnInitialInstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            var name = GetShortcutName();

            CreateShortcut();

            ShowSplashWithRetry(false, $"Installing {name}.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            MessageBox.Show(e.Message, "Initial Install", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    //NOTE: Updated to new version
    private void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            //var name = GetShortcutName();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    //NOTE: Called when the app is no longer the latest version (A new version is installed)
    private void OnAppObsoleted(SemanticVersion version, IAppTools tools)
    {
        try
        {
            //var name = GetShortcutName();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    //NOTE: Called when the app in uninstalled
    private void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        try
        {
            var name = GetShortcutName();

            ShortcutHelper.RemoveShortcut(name);

            ShowSplashWithRetry(false, $"Uninstalling {name}.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    //NOTE: Starts on every run
    private void OnEveryRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        try
        {
            tools.SetProcessAppUserModelId();
            ShowSplashWithRetry(firstRun);
            _ = StartUpdateLoop();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    private void ShowSplashWithRetry(bool firstRun, string entryMessage = null, bool showCloseButton = false)
    {
        try
        {
            ShowSplash(firstRun, entryMessage, showCloseButton);
        }
        catch (InvalidOperationException e)
        {
            Debugger.Break();
            _splash = null;
            UpdateInfoEvent -= ApplicationUpdateStateService_UpdateInfoEvent;
            ShowSplash(firstRun, entryMessage, showCloseButton);
        }
    }

    private void ShowSplash(bool firstRun, string entryMessage, bool showCloseButton)
    {
        if (_splash == null)
        {
            Uri.TryCreate(_applicationLocation, UriKind.Absolute, out var applicationLocation);
            Uri.TryCreate(_applicationLocationSource, UriKind.Absolute, out var applicationSourceLocation);

            var splashData = new SplashData
            {
                MainWindow = _mainWindow,
                FirstRun = firstRun,
                EnvironmentName = _environmentName,
                Version = _version,
                ExeLocation = _exeLocation,
                EntryMessage = entryMessage,
                FullName = _options.ApplicationFullName,
                ClientLocation = applicationLocation,
                ClientSourceLocation = applicationSourceLocation
            };
            _splash = _options.SplashCreator?.Invoke(splashData) ?? new Splash(splashData);
            //UpdateInfoEvent += (_, args) => _splash?.UpdateInfo(args.Message);
            UpdateInfoEvent += ApplicationUpdateStateService_UpdateInfoEvent;
        }

        _splash.ClearMessages();
        if (showCloseButton) _splash.ShowCloseButton();
        _splash.Show();
    }

    private void ApplicationUpdateStateService_UpdateInfoEvent(object sender, UpdateInfoEventArgs e)
    {
        _splash?.UpdateInfo(e.Message);
    }

    private async Task UpdateClientApplication()
    {
        var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
        string clientLocation = null;

        try
        {
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Looking for update."));

            var result = await _applicationDownloadService.GetApplicationLocationAsync();
            clientLocation = result.ApplicationLocation;
            if (string.IsNullOrEmpty(clientLocation))
            {
                var message = $"No application download location configured in options under {nameof(IApplicationDownloadService.GetApplicationLocationAsync)}.";
                _logger.LogWarning(message);
                _splash?.SetErrorMessage(message);
                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            else
            {
                using var mgr = new UpdateManager(clientLocation);
                if (!mgr.IsInstalledApp)
                {
                    var message = Debugger.IsAttached ? $"{_options.ApplicationShortName} is running in debug mode." : $"{_options.ApplicationShortName} is not installed.";
                    //TODO: Start fast when developing... splashDelay = TimeSpan.Zero;
                    //splashDelay = TimeSpan.FromSeconds(10);
                    UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
                    return;
                }

                var updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.CurrentlyInstalledVersion.Version == updateInfo.FutureReleaseEntry.Version)
                {
                    UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Already up to date."));
                    return;
                }

                ShowSplashWithRetry(false);

                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs($"Updating to latest version, {updateInfo.FutureReleaseEntry.Version}."));

                var newVersion = await mgr.UpdateApp();
                if (newVersion != null)
                {
                    await _tabNavigationStateService.CloseAllTabsAsync(true);

                    UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Restarting."));
                    UpdateManager.RestartApp();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            var message = "Update failed. ";
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");
            _splash?.ShowCloseButton();
            //splashDelay = TimeSpan.FromMinutes(5);
        }
        finally
        {
            if (_splash != null)
            {
                if (!_splash.IsCloseButtonVisible)
                {
                    await Task.Delay(splashDelay);
                    _splash?.Close();
                }
            }

            _splash = null;
            UpdateInfoEvent -= ApplicationUpdateStateService_UpdateInfoEvent;
        }
    }

    public void ShowSplash(bool checkForUpdates)
    {
        ShowSplashWithRetry(false, null, true);

        if (checkForUpdates)
        {
            Task.Run(async () =>
            {
                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    await UpdateClientApplication();
                });
            });
        }
    }

    public Task CheckForUpdateAsync()
    {
        return UpdateClientApplication();
    }

    private async Task StartUpdateLoop()
    {
        if (_timer != null)
        {
            if (!_timer.Enabled)
            {
                await UpdateClientApplication();
                _timer.Start();
            }
        }
        else
        {
            await UpdateClientApplication();
        }
    }

    private void CreateShortcut()
    {
        var entryExePath = SquirrelRuntimeInfo.EntryExePath;
        var pos = entryExePath.LastIndexOf("\\", StringComparison.Ordinal);
        var exeName = entryExePath.Substring(pos + 1);

        var baseDirectory = GetDirectory();
        var path = Path.Combine(baseDirectory, exeName);
        var iconPath = Path.Combine(baseDirectory, "app.ico");

        var iconInfo = new ShortcutHelper.IconInfo { Path = iconPath };
        var name = GetShortcutName();
        ShortcutHelper.CreateShortcut(path, name, _options.ApplicationFullName, iconInfo);
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
        return _environmentName == "Production" ? name : $"{name} {_environmentName}";
    }
}