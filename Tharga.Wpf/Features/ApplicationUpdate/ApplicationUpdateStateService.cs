using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal abstract class ApplicationUpdateStateServiceBase : IApplicationUpdateStateService
{
    private readonly ILicenseClient _licenseClient;
    private readonly IApplicationDownloadService _applicationDownloadService;
    private readonly Window _mainWindow;
    private readonly System.Timers.Timer _timer;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _version;
    private readonly ILogger<ApplicationUpdateStateServiceBase> _logger;

    protected readonly ITabNavigationStateService _tabNavigationStateService;
    protected readonly ThargaWpfOptions _options;
    protected readonly string _environmentName;

    /// <summary>
    /// Current splash window. Visible to subclasses so they can show/hide progress and the
    /// close button at the precise moment they know an update is actually starting/finishing.
    /// </summary>
    protected ISplash _splash;
    private string _applicationLocation;
    private string _applicationLocationSource;
    private string _logFileName;
    private bool _checkingForUpdate;
    private bool _isUpdating;
    /// <summary>
    /// True when the splash was opened with an explicit user-visible close button
    /// (e.g. from an About menu). In that case we never hide the close button while
    /// the splash is open, even during an update.
    /// </summary>
    protected bool _persistentCloseButton;

    internal static readonly List<string> UpdateLog = new();

    protected ApplicationUpdateStateServiceBase(IConfiguration configuration, ILoggerFactory loggerFactory, ILicenseClient licenseClient, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
    {
        _licenseClient = licenseClient;
        _applicationDownloadService = applicationDownloadService;
        _tabNavigationStateService = tabNavigationStateService;
        _mainWindow = mainWindow;
        _logger = loggerFactory.CreateLogger<ApplicationUpdateStateServiceBase>();
        _options = options;
        _environmentName = configuration.GetSection("Environment").Value;

        var entryAssembly = Assembly.GetEntryAssembly();
        var assemblyName = entryAssembly?.GetName();
        var version = assemblyName?.Version?.ToString();
        _version = version == "1.0.0.0" ? null : version;

        AddLogString($"Initiate ApplicationUpdateStateService. ({_environmentName} {assemblyName?.FullName})");

        var interval = options.UpdateIntervalCheck;
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
            _timer.Start();
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

        _mainWindow.IsVisibleChanged += (_, _) =>
        {
            if (_mainWindow.Visibility != Visibility.Visible) _splash?.Hide();
        };
    }

    public event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;
    public event EventHandler<LicenseInfoEventArgs> LicenseInfoEvent;
    public event EventHandler<SplashCompleteEventArgs> SplashCompleteEvent;

    protected virtual string ExeLocation { get; } = string.Empty;

    protected void OnUpdateInfoEvent(object sender, string message)
    {
        UpdateInfoEvent?.Invoke(sender, new UpdateInfoEventArgs(message));
    }

    protected abstract Task UpdateAsync(string clientLocation);

    protected void AddLogString(string message)
    {
        var now = DateTime.UtcNow;
        var msg = $"{now:yyyy-MM-dd hh:mm:ss} {message}";

        if (_options.Debug)
        {
            _logFileName ??= $"Log_{now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "_").Replace(":", "")}.txt";
            File.AppendAllLines(_logFileName, [msg]);
        }

        UpdateLog.Add(msg);
    }

    protected async Task ShowSplashWithRetryAsync(bool firstRun, string entryMessage = null, bool showCloseButton = false)
    {
        try
        {
            await ShowSplashAsync(firstRun, entryMessage, showCloseButton);
        }
        catch (InvalidOperationException)
        {
            CloseSplash();
            await ShowSplashAsync(firstRun, entryMessage, showCloseButton);
        }
    }

    private void CloseSplash()
    {
        _logger.LogInformation("CloseSplash (persistentCloseButton was {Persistent}).", _persistentCloseButton);
        _splash?.Close();
        _splash = null;
        _persistentCloseButton = false;
        UpdateInfoEvent -= ApplicationUpdateStateService_UpdateInfoEvent;
    }

    private Task ShowSplashAsync(bool firstRun, string entryMessage, bool showCloseButton)
    {
        _logger.LogInformation("ShowSplashAsync(firstRun={FirstRun}, showCloseButton={ShowCloseButton}, persistentCloseButton={Persistent}, splashExists={SplashExists}).", firstRun, showCloseButton, _persistentCloseButton, _splash != null);

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
                ExeLocation = ExeLocation,
                EntryMessage = entryMessage,
                FullName = _options.ApplicationFullName ?? $"{_options.CompanyName} {_options.ApplicationShortName}".Trim(),
                ClientLocation = applicationLocation,
                ClientSourceLocation = applicationSourceLocation,
                SplashClosed = e =>
                {
                    _persistentCloseButton = false;
                    SplashCompleteEvent?.Invoke(this, new SplashCompleteEventArgs(e, true));
                },
                ImagePath = SplashImageLibrary.TealTransparent
            };
            var app = Application.Current;
            var dispatcher = app?.Dispatcher;
            if (dispatcher == null)
            {
                _logger.LogError("Cannot create splash - Application.Current.Dispatcher is null. Calling thread ApartmentState={ApartmentState}.", Thread.CurrentThread.GetApartmentState());
                return Task.CompletedTask;
            }

            _splash = dispatcher.Invoke(() =>
            {
                _logger.LogInformation("Creating splash on thread ApartmentState={ApartmentState}, IsDispatcherThread={IsDispatcherThread}.", Thread.CurrentThread.GetApartmentState(), dispatcher.CheckAccess());
                return _options.SplashCreator?.Invoke(splashData) ?? new Splash(splashData);
            });
            UpdateInfoEvent += ApplicationUpdateStateService_UpdateInfoEvent;
        }

        _splash.HideProgress();
        if (showCloseButton || _persistentCloseButton)
            _splash.ShowCloseButton();
        else
            _splash.HideCloseButton();
        _splash.Show();
        return Task.CompletedTask;
    }

    private void ApplicationUpdateStateService_UpdateInfoEvent(object sender, UpdateInfoEventArgs e)
    {
        AddLogString($"{e.Message}");
        _splash?.UpdateInfo(e.Message);
    }

    private async Task UpdateClientApplication(string source)
    {
        string clientLocation = null;

        try
        {
            if (_checkingForUpdate)
            {
                AddLogString($"Ignore {nameof(UpdateClientApplication)} since it is already running. (source: {source})");
                _logger.LogInformation("UpdateClientApplication ignored - already running. source={Source}", source);
                return;
            }

            await _lock.WaitAsync();
            _checkingForUpdate = true;

            AddLogString($"--- Start {nameof(UpdateClientApplication)} (source: {source}) ---");
            _logger.LogInformation("UpdateClientApplication start. source={Source}, persistentCloseButton={Persistent}", source, _persistentCloseButton);

            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Looking for update."));

            if (Debugger.IsAttached)
            {
                var message = $"{_options.ApplicationShortName} is running in debug mode.";
                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
                _logger.LogInformation("UpdateClientApplication skipped - debugger attached.");
                return;
            }

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
                AddLogString($"locationSource: {result.ApplicationLocationSource}");
                AddLogString($"clientLocation: {clientLocation}");
                _logger.LogInformation("UpdateClientApplication invoking UpdateAsync. clientLocation={ClientLocation}", clientLocation);

                _isUpdating = true;
                await ShowSplashWithRetryAsync(false, null, _persistentCloseButton);

                // The close button + progress visibility during the update is the subclass's
                // responsibility (it knows whether an update is actually being downloaded vs
                // "already up to date"). The persistent close button is honoured by
                // ShowSplashAsync above, so the subclass should NOT hide it when persistent.
                await UpdateAsync(clientLocation);
            }
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
			_logger.LogError(e, e.Message);
            var message = "Update failed. ";
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            _splash?.HideProgress();
            _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");
            _splash?.ShowCloseButton();
        }
        finally
        {
            SplashCompleteEvent?.Invoke(this, new SplashCompleteEventArgs(CloseMethod.None, false));

            //if (_splash != null && !_splash.IsCloseButtonVisible)
            //{
            //    await Task.Delay(splashDelay);
            //    CloseSplash();
            //}

            //AddLogString($"Complete check for updates.");
            AddLogString($"--- End {nameof(UpdateClientApplication)} ---");
            _isUpdating = false;
            _checkingForUpdate = false;
			_lock.Release();
        }
    }

    public async Task ShowSplashAsync(bool checkForUpdates, bool showCloseButton, bool checkForLicense)
    {
        _logger.LogInformation("ShowSplashAsync(checkForUpdates={CheckForUpdates}, showCloseButton={ShowCloseButton}, checkForLicense={CheckForLicense}).", checkForUpdates, showCloseButton, checkForLicense);
        if (showCloseButton) _persistentCloseButton = true;

        await ShowSplashWithRetryAsync(false, null, showCloseButton);
        if (checkForUpdates) await CheckForUpdateAsync($"{nameof(ShowSplashAsync)}");
        if (checkForLicense) await CheckForLicenseAsync($"{nameof(ShowSplashAsync)}");

        if (!showCloseButton && !_isUpdating)
        {
            var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
            await Task.Delay(splashDelay);
            if (_splash != null && !_splash.IsCloseButtonVisible && !_isUpdating)
            {
                CloseSplash();
            }
        }
    }

    public Task CheckForUpdateAsync(string source)
    {
        return UpdateClientApplication($"Call from {source ?? "Unknown"}");
    }

    public async Task<bool> CheckForLicenseAsync(string source = null)
    {
        UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Looking for license."));
        var isValid = false;
        var message = string.Empty;

        try
        {
            if (Debugger.IsAttached)
            {
                message = "No license needed.";
                isValid = true;
                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            }
            else
            {
                var result = await _licenseClient.CheckLicenseAsync();
                if (!result.IsValid)
                {
                    _splash?.SetErrorMessage(result.Message);
                    _splash?.ShowCloseButton();
                }

                message = result.Message;
                isValid = result.IsValid;
                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            }
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
            _logger.LogError(e, e.Message);
            _splash?.SetErrorMessage($"{e.Message}\n{source}\n@{e.StackTrace})");
            _splash?.ShowCloseButton();
            message = e.Message;
            isValid = false;
        }
        finally
        {
            LicenseInfoEvent?.Invoke(this, new LicenseInfoEventArgs(isValid, message));
        }

        return isValid;
    }
}