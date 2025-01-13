using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Squirrel;
using Tharga.Toolkit;
using Tharga.Wpf.TabNavigator;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Tharga.Wpf.ApplicationUpdate;

internal class ApplicationUpdateStateService : IApplicationUpdateStateService
{
    private readonly IApplicationDownloadService _applicationDownloadService;
    private readonly ITabNavigationStateService _tabNavigationStateService;
    private readonly ThargaWpfOptions _options;
    private readonly Window _mainWindow;
    private readonly ILogger<ApplicationUpdateStateService> _logger;
    private readonly System.Timers.Timer _timer;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _environmentName;
    private readonly string _version;
    private readonly string _exeLocation;
    private ISplash _splash;
    private string _applicationLocation;
    private string _applicationLocationSource;
    private string _logFileName;
    private bool _checkingForUpdate;

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
        _exeLocation = SquirrelRuntimeInfo.EntryExePath;

        AddLogString($"Initiate ApplicationUpdateStateService. ({_environmentName} {assemblyName?.FullName})");

        var interval = options.CheckForUpdateInterval;
        if (interval != null && interval > TimeSpan.Zero)
        {
            _timer = new System.Timers.Timer { AutoReset = true, Enabled = false, Interval = interval.Value.TotalMilliseconds };
            _timer.Elapsed += async (_, _) =>
            {
                try
                {
                    await UpdateClientApplication("Timer elapsed");
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

        SquirrelAwareApp.HandleEvents(OnInitialInstall, OnAppInstall, OnAppObsoleted, OnAppUninstall, OnEveryRun);

        _mainWindow.IsVisibleChanged += (_, _) =>
        {
            if (_mainWindow.Visibility != Visibility.Visible) _splash?.Hide();
        };
    }

    public event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    public event EventHandler<SplashCompleteEventArgs> SplashCompleteEvent;

    internal static readonly List<string> UpdateLog = new();

    private void AddLogString(string message)
    {
        var now = DateTime.UtcNow;
        var msg = $"{now:yyyy-MM-dd hh:mm:ss} {message}";

        if (_options.Debug)
        {
            _logFileName ??= $"Log_{now.ToLocalDateTimeString().Replace(" ", "_").Replace(":", "")}.txt";
            File.AppendAllLines(_logFileName, [msg]);
        }

        UpdateLog.Add(msg);
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

		    ShowSplashWithRetry(false, $"Installing {name}.");
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

		    ShowSplashWithRetry(false, $"Uninstalling {name}.");
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
		    ShowSplashWithRetry(firstRun);
		    _ = StartUpdateLoop();
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

    private void ShowSplashWithRetry(bool firstRun, string entryMessage = null, bool showCloseButton = false)
    {
        try
        {
            ShowSplash(firstRun, entryMessage, showCloseButton);
        }
        catch (InvalidOperationException)
        {
            CloseSplash();
            ShowSplash(firstRun, entryMessage, showCloseButton);
        }
    }

    private void CloseSplash()
    {
        _splash?.Close();
        _splash = null;
        UpdateInfoEvent -= ApplicationUpdateStateService_UpdateInfoEvent;
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
                FullName = _options.ApplicationFullName ?? $"{_options.CompanyName} {_options.ApplicationShortName}".Trim(),
                ClientLocation = applicationLocation,
                ClientSourceLocation = applicationSourceLocation,
                SplashClosed = e => { SplashCompleteEvent?.Invoke(this, new SplashCompleteEventArgs(e, true)); }
            };
            _splash = _options.SplashCreator?.Invoke(splashData) ?? new Splash(splashData);
            UpdateInfoEvent += ApplicationUpdateStateService_UpdateInfoEvent;
        }

        //_splash.ClearMessages();
        if (showCloseButton) _splash.ShowCloseButton();
        _splash.Show();
    }

    private void ApplicationUpdateStateService_UpdateInfoEvent(object sender, UpdateInfoEventArgs e)
    {
        AddLogString($"{e.Message}");
        _splash?.UpdateInfo(e.Message);
    }

    private async Task UpdateClientApplication(string source)
    {
        var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
        string clientLocation = null;

        try
        {
            if (_checkingForUpdate)
            {
                AddLogString($"Ignore {nameof(UpdateClientApplication)} since it is already running. (source: {source})");
                //UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs($"Ignore update from {source} since it is already running."));
                return;
            }

            await _lock.WaitAsync();
            _checkingForUpdate = true;

            AddLogString($"--- Start {nameof(UpdateClientApplication)} (source: {source}) ---");

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
                AddLogString($"clientLocation: {clientLocation}");
                using var mgr = new UpdateManager(clientLocation);
                if (!mgr.IsInstalledApp)
                {
                    var message = Debugger.IsAttached ? $"{_options.ApplicationShortName} is running in debug mode." : $"{_options.ApplicationShortName} is not installed.";
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
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
			_logger.LogError(e, e.Message);
            var message = "Update failed. ";
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");
            _splash?.ShowCloseButton();
        }
        finally
        {
            SplashCompleteEvent?.Invoke(this, new SplashCompleteEventArgs(CloseMethod.None, false));

            if (_splash != null && !_splash.IsCloseButtonVisible)
            {
                await Task.Delay(splashDelay);
                CloseSplash();
            }

            //AddLogString($"Complete check for updates.");
            AddLogString($"--- End {nameof(UpdateClientApplication)} ---");
            _checkingForUpdate = false;
			_lock.Release();
        }
    }

    public void ShowSplash(bool checkForUpdates, bool showCloseButton)
    {
        ShowSplashWithRetry(false, null, showCloseButton);

        if (checkForUpdates)
        {
            Task.Run(async () =>
            {
                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    await UpdateClientApplication($"{nameof(ShowSplash)}");
                });
            });
        }
    }

    public Task CheckForUpdateAsync(string source)
    {
        return UpdateClientApplication($"Call from {source}");
    }

    private async Task StartUpdateLoop()
    {
        if (_timer != null)
        {
            if (!_timer.Enabled)
            {
                await UpdateClientApplication($"{nameof(StartUpdateLoop)} before timer");
                _timer.Start();
            }
        }
        else
        {
            await UpdateClientApplication($"{nameof(StartUpdateLoop)} no timer");
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