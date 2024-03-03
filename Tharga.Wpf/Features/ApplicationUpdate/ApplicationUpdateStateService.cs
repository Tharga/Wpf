using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tharga.Wpf.Features.ApplicationUpdate;

internal class ApplicationUpdateStateService : IApplicationUpdateStateService
{
//    private readonly ICommunicationService _communicationService;
//    private readonly ITabNavigationStateService _tabNavigationService;
    private readonly ILogger _logger;
    private readonly System.Timers.Timer _timer;
    private readonly string _environmentName;
//    private readonly string _version;
//    private Splash _splash;
    private Window _mainWindow;
//    private int _checkCounter;

    public ApplicationUpdateStateService(IConfiguration configuration, ILogger logger) //, ICommunicationService communicationService, ITabNavigationStateService tabNavigationService)
    {
//        _communicationService = communicationService;
//        _tabNavigationService = tabNavigationService;
        _logger = logger;
        _environmentName = configuration.GetSection("Environment").Value;
//        _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

//        SquirrelAwareApp.HandleEvents(OnInitialInstall, OnAppInstall, OnAppObsoleted, OnAppUninstall, OnEveryRun);

        var interval = _environmentName == "Production" ? TimeSpan.FromHours(1) : TimeSpan.FromMinutes(1);
        _timer = new System.Timers.Timer { AutoReset = true, Enabled = false, Interval = interval.TotalMilliseconds };
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

    public event EventHandler<UpdateInfoEventArgs> UpdateInfoEvent;

    //    //NOTE: Initial Install
    //    private void OnInitialInstall(SemanticVersion version, IAppTools tools)
    //    {
    //        try
    //        {
    //            var name = GetShortcutName();

    //            CreateShortcut();

    //            ShowSplash(false, $"Installerar {name}.");
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e, e.Message);
    //            MessageBox.Show(e.Message, "Initial Install", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //    }

    //    //NOTE: Updated to new version
    //    private void OnAppInstall(SemanticVersion version, IAppTools tools)
    //    {
    //        try
    //        {
    //            var name = GetShortcutName();
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e, e.Message);
    //        }
    //    }

    //    //NOTE: Called when the app is no longer the latest version (A new version is installed)
    //    private void OnAppObsoleted(SemanticVersion version, IAppTools tools)
    //    {
    //        try
    //        {
    //            var name = GetShortcutName();
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e, e.Message);
    //        }
    //    }

    //    //NOTE: Called when the app in uninstalled
    //    private void OnAppUninstall(SemanticVersion version, IAppTools tools)
    //    {
    //        try
    //        {
    //            var name = GetShortcutName();

    //            RemoveShortcut(name);

    //            ShowSplash(false, $"Avinstallerar {name}.");
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e, e.Message);
    //        }
    //    }

    //    //NOTE: Starts on every run
    //    private void OnEveryRun(SemanticVersion version, IAppTools tools, bool firstRun)
    //    {
    //        try
    //        {
    //            tools.SetProcessAppUserModelId();
    //            ShowSplash(firstRun);
    //        }
    //        catch (Exception e)
    //        {
    //            _logger.LogError(e, e.Message);
    //        }
    //    }

    //    private void ShowSplash(bool firstRun, string entryMessage = null)
    //    {
    //        if (_splash != null) return;
    //        _splash = new Splash(_mainWindow, firstRun, _environmentName, _version, entryMessage);
    //        UpdateInfoEvent += (_, args) => _splash?.UpdateInfo(args.Message);
    //        _splash.Show();
    //    }

    private async Task UpdateClientApplication()
    {
        //        var splashDelay = TimeSpan.FromSeconds(2);
        //        string clientLocation = null;
        //        _checkCounter++;

        //        try
        //        {
        //            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Letar efter uppdatering."));

        //            var response = await _communicationService.GetAsync<HealthCheck>(EController.HealthCheck);
        //            clientLocation = response.ClientLocation;

        //            using var mgr = new UpdateManager(clientLocation);
        //            if (!mgr.IsInstalledApp)
        //            {
        //                var message = Debugger.IsAttached ? $"Florida körs i debuggläge ({_checkCounter})." : $"Florida är inte installerat ({_checkCounter}).";
        //                //TODO: Start fast when developing... splashDelay = TimeSpan.Zero;
        //                //splashDelay = TimeSpan.FromSeconds(10);
        //                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
        //                return;
        //            }

        //            var updateInfo = await mgr.CheckForUpdate();
        //            if (updateInfo.CurrentlyInstalledVersion.Version == updateInfo.FutureReleaseEntry.Version)
        //            {
        //                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs($"Senaste versionen är installerad ({_checkCounter})."));
        //                return;
        //            }

        //            ShowSplash(false);

        //            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs($"Uppdaterar till version {updateInfo.FutureReleaseEntry.Version}."));

        //            var newVersion = await mgr.UpdateApp();
        //            if (newVersion != null)
        //            {
        //                await _tabNavigationService.CloseAllTabsAsync(true);

        //                UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs("Startar om."));
        //                UpdateManager.RestartApp();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.LogError(e, e.Message);
        //            var message = $"Uppdateringen misslyckades ({_checkCounter}).";
        //            UpdateInfoEvent?.Invoke(this, new UpdateInfoEventArgs(message));
        //            _splash?.SetErrorMessage($"{e.Message}\n{clientLocation}\n@{e.StackTrace})");

        //            _splash?.ShowCloseButton();
        //            splashDelay = TimeSpan.FromMinutes(5);
        //        }
        //        finally
        //        {
        //            if (_splash != null)
        //            {
        //                await Task.Delay(splashDelay);
        //                _splash?.Close();
        //            }
        //            _splash = null;
        //        }
        Debugger.Break();
        throw new NotImplementedException();
    }

    public async Task StartUpdateLoop()
    {
        if (!_timer.Enabled)
        {
            await UpdateClientApplication();
            _timer.Start();
        }
    }

    public void AttachMainWindow(Window mainWindow)
    {
        _mainWindow = mainWindow;
        //_splash?.SetOwner(_mainWindow); //TODO: Implement
    }

    //    private void CreateShortcut()
    //    {
    //        var baseDirectory = GetDirectory();
    //        var path = Path.Combine(baseDirectory, "Tharga.Florida.Client.exe");
    //        var iconPath = Path.Combine(baseDirectory, "app.ico");

    //        var iconInfo = new IconInfo { Path = iconPath };
    //        var name = GetShortcutName();
    //        ShortcutHelper.CreateShortcut(path, name, "Florida kassa", iconInfo);
    //    }

    //    private static string GetDirectory()
    //    {
    //        var baseDirectory = SquirrelRuntimeInfo.BaseDirectory;
    //        var pos = baseDirectory.TrimEnd('\\').LastIndexOf("\\", StringComparison.Ordinal);
    //        baseDirectory = baseDirectory.Substring(0, pos);
    //        return baseDirectory;
    //    }

    //    private string GetShortcutName()
    //    {
    //        return _environmentName == "Production" ? "Florida" : $"Florida {_environmentName}";
    //    }
}