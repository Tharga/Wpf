﻿using System.Diagnostics;
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
    private readonly SemaphoreSlim _lock = new(1, 1);
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
        _exeLocation = SquirrelRuntimeInfo.EntryExePath;

        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Initiate ApplicationUpdateStateService. ({_environmentName} {_version}, {assemblyName?.FullName})");

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
    public event EventHandler<EventArgs> SplashClosedEvent;

    internal static readonly List<string> UpdateLog = new();

    //NOTE: Initial Install
    private void OnInitialInstall(SemanticVersion version, IAppTools tools)
    {
	    try
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(OnInitialInstall)} ---");

		    var name = GetShortcutName();

		    CreateShortcut();

		    ShowSplashWithRetry(false, $"Installing {name}.");
	    }
	    catch (Exception e)
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
		    _logger.LogError(e, e.Message);
		    MessageBox.Show(e.Message, "Initial Install", MessageBoxButton.OK, MessageBoxImage.Error);
	    }
	    finally
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(OnInitialInstall)} ---");
		}
	}

    //NOTE: Updated to new version
    private void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
	    try
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(OnAppInstall)} ---");

		    //var name = GetShortcutName();
	    }
	    catch (Exception e)
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
		    _logger.LogError(e, e.Message);
	    }
	    finally
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(OnAppInstall)} ---");
		}
	}

    //NOTE: Called when the app is no longer the latest version (A new version is installed)
    private void OnAppObsoleted(SemanticVersion version, IAppTools tools)
    {
	    try
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(OnAppObsoleted)} ---");

		    //var name = GetShortcutName();
	    }
	    catch (Exception e)
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
		    _logger.LogError(e, e.Message);
	    }
	    finally
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(OnAppObsoleted)} ---");
		}
	}

    //NOTE: Called when the app in uninstalled
    private void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
	    try
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(OnAppUninstall)} ---");

		    var name = GetShortcutName();

		    ShortcutHelper.RemoveShortcut(name);

		    ShowSplashWithRetry(false, $"Uninstalling {name}.");
	    }
	    catch (Exception e)
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
		    _logger.LogError(e, e.Message);
	    }
	    finally
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(OnAppUninstall)} ---");
		}
	}

    //NOTE: Starts on every run
    private void OnEveryRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
	    try
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(OnEveryRun)} ---");

		    tools.SetProcessAppUserModelId();
		    ShowSplashWithRetry(firstRun);
		    _ = StartUpdateLoop();
	    }
	    catch (Exception e)
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
		    _logger.LogError(e, e.Message);
	    }
	    finally
	    {
		    UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(OnEveryRun)} ---");
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
                SplashClosed = () => { SplashClosedEvent?.Invoke(this, EventArgs.Empty); }
            };
            _splash = _options.SplashCreator?.Invoke(splashData) ?? new Splash(splashData);
            UpdateInfoEvent += ApplicationUpdateStateService_UpdateInfoEvent;
        }

        _splash.ClearMessages();
        if (showCloseButton) _splash.ShowCloseButton();
        _splash.Show();
    }

    private void ApplicationUpdateStateService_UpdateInfoEvent(object sender, UpdateInfoEventArgs e)
    {
        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} {e.Message}");
        _splash?.UpdateInfo(e.Message);
    }

    private async Task UpdateClientApplication(string source)
    {
        var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
        string clientLocation = null;

        try
        {
            await _lock.WaitAsync();

			//UpdateLog.Add("--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---");
			//UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Start check for updates. {source}");
			UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- Start {nameof(UpdateClientApplication)} ---");

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
                //using var mgr = new UpdateManager(clientLocation,"C:\\Users\\danie\\AppData\\Local\\EplictaAgentWpfCI\\"); //TODO: Provide something here that might help the manager to find the currently installed version.
                //var ver = mgr.CurrentlyInstalledVersion($"C:\\Users\\danie\\AppData\\Local\\EplictaAgentWpfCI\\Eplicta.Agent.Wpf.exe");
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
	        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Error: {e.Message} @{e.StackTrace}");
			_logger.LogError(e, e.Message);
            var message = "Update failed. ";
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");
            _splash?.ShowCloseButton();
        }
        finally
        {
            if (_splash != null && !_splash.IsCloseButtonVisible)
            {
                await Task.Delay(splashDelay);
                CloseSplash();
            }

			//UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Complete check for updates.");
			UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} --- End {nameof(UpdateClientApplication)} ---");
			_lock.Release();
        }
    }

    public void ShowSplash(bool checkForUpdates, bool autoClose = true)
    {
        ShowSplashWithRetry(false, null, !autoClose);

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
        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Exe name is '{exeName}'.");

		var baseDirectory = GetDirectory();
        var path = Path.Combine(baseDirectory, exeName);
        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Path is '{path}'.");

		var iconPath = Path.Combine(baseDirectory, "app.ico");
		UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} IconPath is '{iconPath}'.");

		var iconInfo = new ShortcutHelper.IconInfo { Path = iconPath };
        var name = GetShortcutName();
        var description = _options.ApplicationFullName ?? $"{_options.CompanyName} {_options.ApplicationShortName}".Trim();
        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Description is '{description}'.");

		ShortcutHelper.CreateShortcut(path, name, description, iconInfo);
        UpdateLog.Add($"{DateTime.UtcNow:yyyy-MM-dd hh:mm:ss} Shortcut created with name '{name}'.");
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