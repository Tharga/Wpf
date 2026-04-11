using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Tharga.License;
using Tharga.Runtime;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.ExceptionHandling;
using Tharga.Wpf.Framework;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.WindowLocation;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf;

/// <summary>
/// Abstract base class for WPF applications using the Tharga toolkit.
/// Provides dependency injection, exception handling, tab navigation, and update management.
/// </summary>
public abstract class ApplicationBase : Application
{
    private readonly CancellationService _cs;
    private readonly ThargaWpfOptions _options;
    private Mutex _mutex;

    /// <summary>
    /// Raised before the application closes, allowing handlers to cancel the operation.
    /// </summary>
    public static event EventHandler<BeforeCloseEventArgs> BeforeCloseEvent;

    /// <summary>
    /// Initializes the application host, dependency injection, and event handlers.
    /// </summary>
    protected ApplicationBase()
    {
        DispatcherUnhandledException += async (_, e) =>
        {
            try
            {
                var ess = GetService<IExceptionStateService>();
                await ess.FallbackHandlerInternalAsync(e.Exception);
                e.Handled = true;
            }
            catch (Exception exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
                Debugger.Break();
            }
        };

        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        _cs = new CancellationService();
        _options = new ThargaWpfOptions
        {
            ApplicationFullName = assemblyName?.Name ?? "Unknown Application",
            ApplicationShortName = assemblyName?.Name ?? "Unknown",
        };

        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                Options(_options);

                services.AddSingleton(_ => _options);
                services.AddHttpClient();

                RegisterExceptionHandler(_options, services);
                RegisterTabNavigation(_options, services);
                RegisterIconTray(_options, services);

                var assemblies = AssemblyService.GetAssemblies().Union(_options.GetAssemblies().Values);
                var viewModels = AssemblyService.GetTypes<IViewModel>(x => !x.IsAbstract && !x.IsInterface, assemblies).Select(x => x.AsType());
                foreach (var viewModel in viewModels)
                {
                    services.AddTransient(viewModel);
                }

                services.AddSingleton<ICancellationService>(_ => _cs);

                services.AddSingleton<IWindowLocationService>(s =>
                {
                    var loggerFactory = s.GetService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<WindowLocationService>();
                    return new WindowLocationService(_options, logger);
                });

                services.AddSingleton<IApplicationUpdateStateService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var applicationDownloadService = s.GetService<IApplicationDownloadService>();
                    var tabNavigationStateService = s.GetService<ITabNavigationStateService>();
                    var mainWindow = ((ApplicationBase)Current).MainWindow;
                    var loggerFactory = s.GetService<ILoggerFactory>();
                    var licenseClient = s.GetService<ILicenseClient>() ?? throw new NullReferenceException();
                    //var logger = loggerFactory.CreateLogger<ApplicationUpdateStateService>();
                    //return new ApplicationUpdateStateService(configuration, applicationDownloadService, tabNavigationStateService, _options, mainWindow, logger);

                    switch (_options.UpdateSystem)
                    {
                        case UpdateSystem.None:
                            return new NoUpdateStateServiceBase(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, _options, mainWindow);
                        case UpdateSystem.Squirrel:
                            return new SquirrelApplicationUpdateStateService(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, _options, mainWindow);
                        case UpdateSystem.Velopack:
                            return new VelopackApplicationUpdateStateService(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, _options, mainWindow);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_options.UpdateSystem), @$"Unknown {nameof(_options.UpdateSystem)} {_options.UpdateSystem}.");
                    }
                });
                services.AddTransient<IApplicationDownloadService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var httpClientFactory = s.GetService<IHttpClientFactory>();
                    return new ApplicationDownloadService(configuration, httpClientFactory, _options);
                });

                services.AddTransient<ILicenseClient, LicenseClient>();
                services.AddThargaLicense();

                //if (_options.Inactivity?.Timeout > TimeSpan.Zero)
                //{
                //    _ = new InactivityService(_options.Inactivity);
                //}

                StaticExceptionHandler.ErrorEvent += (s, e) =>
                {
                    var srv = services.BuildServiceProvider().GetService<IExceptionStateService>();
                    srv.FallbackHandlerInternalAsync(e.Exception);
                };

                Register(context, services);
            })
            .Build();
    }

    /// <summary>
    /// Gets the application host providing access to dependency injection and configuration.
    /// </summary>
    public IHost AppHost { get; protected init; }

    private static void RegisterExceptionHandler(ThargaWpfOptions options, IServiceCollection services)
    {
        var exceptionHandlers = options.GetExceptionTypes();
        foreach (var exceptionHandler in exceptionHandlers)
        {
            services.AddTransient(exceptionHandler.Value);
        }

        foreach (var exceptionHandlerServices in options.GetExceptionHandlerServices())
        {
            services.AddTransient(exceptionHandlerServices.Value);
        }

        services.AddSingleton<IExceptionStateService>(s =>
        {
            var loggerFactory = s.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ExceptionStateService>();
            var mainWindow = ((ApplicationBase)Current).MainWindow;
            return new ExceptionStateService(s, mainWindow, logger, exceptionHandlers);
        });
    }

    private static void RegisterTabNavigation(ThargaWpfOptions options, IServiceCollection services)
    {
        services.AddTransient<TabNavigatorViewModel>();
        services.AddSingleton<ITabNavigationStateService>(s => new TabNavigationStateService(options, s));
        foreach (var tabViewTab in TypeHelper.GetTypesBasedOn<TabView>())
        {
            services.AddTransient(tabViewTab);
        }
    }

    private static void RegisterIconTray(ThargaWpfOptions options, IServiceCollection services)
    {
        services.AddSingleton<INotifyIconService>(_ => new NotifyIconService(options));
    }

    /// <summary>
    /// Override to register additional services in the dependency injection container.
    /// </summary>
    /// <param name="context">The host builder context.</param>
    /// <param name="services">The service collection to register services with.</param>
    protected virtual void Register(HostBuilderContext context, IServiceCollection services) { }

    /// <summary>
    /// Override to configure application options.
    /// </summary>
    /// <param name="thargaWpfOptions">The options to configure.</param>
    protected virtual void Options(ThargaWpfOptions thargaWpfOptions) { }

    /// <inheritdoc />
    protected override async void OnStartup(StartupEventArgs args)
    {
        try
        {
            if (!_options.AllowMultipleApplications)
            {
                _mutex = new Mutex(true, _options.ApplicationFullName, out var createdNew);
                if (!createdNew)
                {
                    BringExistingInstanceToFront();
                    Shutdown();
                    return;
                }
            }

            await AppHost.StartAsync();
            base.OnStartup(args);
        }
        catch (Exception e)
        {
            Debugger.Break();
            Trace.TraceError($"{e.Message} @{e.StackTrace}");
        }
    }

    /// <inheritdoc />
    protected override async void OnExit(ExitEventArgs args)
    {
        try
        {
            await _cs.CancelAsync();
            _mutex?.Close();
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(args);
        }
        catch (Exception e)
        {
            Debugger.Break();
            Trace.TraceError($"{e.Message} @{e.StackTrace}");
        }
    }

    /// <summary>
    /// Resolves a service of type <typeparamref name="T"/> from the dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is not registered.</exception>
    public static T GetService<T>()
    {
        var service = ((ApplicationBase)Current).AppHost.Services.GetService<T>();
        if (service == null) throw new InvalidOperationException($"Cannot find service '{typeof(T).Name}'. Perhaps it has not been registered in the IOC.");
        return service;
    }

    /// <summary>
    /// Call at the start of OnClosing to let the framework handle hide-on-close.
    /// Returns true if the close was handled (window hidden) and the caller should return.
    /// </summary>
    /// <param name="e">The closing event args.</param>
    /// <returns><c>true</c> if the close was handled (hidden to tray); <c>false</c> if closing should proceed.</returns>
    public static bool HandleClose(System.ComponentModel.CancelEventArgs e)
    {
        var options = ((ApplicationBase)Current)._options;
        if (options.HideOnClose && CloseMode == CloseMode.Default)
        {
            Hide();
            e.Cancel = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Hides the main window to the system tray and saves its visibility state.
    /// </summary>
    public static void Hide()
    {
        var mainWindow = Current?.MainWindow;
        if (mainWindow == null) return;

        foreach (var owned in mainWindow.OwnedWindows.Cast<System.Windows.Window>())
        {
            owned.Hide();
        }

        mainWindow.Hide();

        var windowLocationService = ((ApplicationBase)Current).AppHost.Services.GetService<IWindowLocationService>();
        var name = string.IsNullOrEmpty(mainWindow.Name) ? mainWindow.GetType().Name : mainWindow.Name;
        windowLocationService?.SetVisibility(name, mainWindow.Visibility);
    }

    /// <summary>
    /// Closes the application with the specified close mode.
    /// </summary>
    /// <param name="closeMode">The close mode to use.</param>
    /// <returns><c>true</c> if the close was successful; <c>false</c> if it was cancelled.</returns>
    public static bool Close(CloseMode closeMode = CloseMode.Default)
    {
        try
        {
            var options = ((ApplicationBase)Current)._options;
            var action = CloseActionResolver.Resolve(closeMode, options.HideOnClose);

            if (action == CloseAction.Hide)
            {
                Hide();
                return true;
            }

            CloseMode = closeMode;

            var beforeCloseEventArgs = new BeforeCloseEventArgs();
            BeforeCloseEvent?.Invoke(null, beforeCloseEventArgs);
            if (beforeCloseEventArgs.Cancel) return false;

            Current?.MainWindow?.Close();
        }
        catch (Exception e)
        {
            Debugger.Break();
            Trace.TraceError($"{e.Message} @{e.StackTrace}");
            throw;
        }
        finally
        {
            CloseMode = CloseMode.Default;
        }

        return true;
    }

    private void BringExistingInstanceToFront()
    {
        var currentProcess = Process.GetCurrentProcess();
        var processes = Process.GetProcessesByName(currentProcess.ProcessName);
        var process = processes.FirstOrDefault(x => x.Id != currentProcess.Id);
        if (process != null) WindowHelper.FocusWindowByProcessId(process.Id);
    }

    /// <summary>
    /// Gets the current close mode of the application.
    /// </summary>
    public static CloseMode CloseMode { get; private set; }
}