using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Tharga.Toolkit;
using Tharga.Wpf.TabNavigator;
using Application = System.Windows.Application;

namespace Tharga.Wpf.ApplicationUpdate;

internal abstract class ApplicationUpdateStateServiceBase : IApplicationUpdateStateService
{
    private readonly IApplicationDownloadService _applicationDownloadService;
    private readonly Window _mainWindow;
    private readonly System.Timers.Timer _timer;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _version;
    private readonly ILogger<ApplicationUpdateStateServiceBase> _logger;

    protected readonly ITabNavigationStateService _tabNavigationStateService;
    protected readonly ThargaWpfOptions _options;
    protected readonly string _environmentName;

    private ISplash _splash;
    private string _applicationLocation;
    private string _applicationLocationSource;
    private string _logFileName;
    private bool _checkingForUpdate;

    internal static readonly List<string> UpdateLog = new();

    protected ApplicationUpdateStateServiceBase(IConfiguration configuration, ILoggerFactory loggerFactory, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
    {
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
            _logFileName ??= $"Log_{now.ToLocalDateTimeString().Replace(" ", "_").Replace(":", "")}.txt";
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
        _splash?.Close();
        _splash = null;
        UpdateInfoEvent -= ApplicationUpdateStateService_UpdateInfoEvent;
    }

    private async Task ShowSplashAsync(bool firstRun, string entryMessage, bool showCloseButton)
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
                ExeLocation = ExeLocation,
                EntryMessage = entryMessage,
                FullName = _options.ApplicationFullName ?? $"{_options.CompanyName} {_options.ApplicationShortName}".Trim(),
                ClientLocation = applicationLocation,
                ClientSourceLocation = applicationSourceLocation,
                SplashClosed = e => { SplashCompleteEvent?.Invoke(this, new SplashCompleteEventArgs(e, true)); },
                ImagePath = SplashImageLibrary.Teal
            };
            _splash = _options.SplashCreator?.Invoke(splashData) ?? new Splash(splashData);
            UpdateInfoEvent += ApplicationUpdateStateService_UpdateInfoEvent;
        }

        //_splash.ClearMessages();
        if (showCloseButton) _splash.ShowCloseButton();
        _splash.Show();

        //var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
        //await Task.Delay(splashDelay);
        //if (_splash != null && !_splash.IsCloseButtonVisible)
        //{
        //    CloseSplash();
        //}
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
                //UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs($"Ignore update from {source} since it is already running."));
                return;
            }

            await _lock.WaitAsync();
            _checkingForUpdate = true;

            AddLogString($"--- Start {nameof(UpdateClientApplication)} (source: {source}) ---");

            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Looking for update."));

            if (Debugger.IsAttached)
            {
                var message = $"{_options.ApplicationShortName} is running in debug mode.";
                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
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

                await UpdateAsync(clientLocation);
            }
        }
        catch (Exception e)
        {
            AddLogString($"Error: {e.Message} @{e.StackTrace}");
			_logger.LogError(e, e.Message);
            var message = "Update failed. ";
            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
            Application.Current.Dispatcher.Invoke(() =>
            {
                _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");
                _splash?.ShowCloseButton();
            });
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
            _checkingForUpdate = false;
			_lock.Release();
        }
    }

    public async Task ShowSplashAsync(bool checkForUpdates, bool showCloseButton)
    {
        await ShowSplashWithRetryAsync(false, null, showCloseButton);
        if (checkForUpdates) await UpdateClientApplication($"{nameof(ShowSplashAsync)}");

        if (!showCloseButton)
        {
            var splashDelay = Debugger.IsAttached ? TimeSpan.FromSeconds(4) : TimeSpan.FromSeconds(2);
            await Task.Delay(splashDelay);
            if (_splash != null && !_splash.IsCloseButtonVisible)
            {
                CloseSplash();
            }
        }
    }

    public Task CheckForUpdateAsync(string source)
    {
        return UpdateClientApplication($"Call from {source ?? "Unknown"}");
    }
}