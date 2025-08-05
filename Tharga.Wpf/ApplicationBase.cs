using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Tharga.License;
using Tharga.Toolkit.TypeService;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.ExceptionHandling;
using Tharga.Wpf.Framework;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf;

public abstract class ApplicationBase : Application
{
    private readonly CancellationService _cs;
    private readonly ThargaWpfOptions _options;
    private Mutex _mutex;

    public static event EventHandler<BeforeCloseEventArgs> BeforeCloseEvent;

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

    protected virtual void Register(HostBuilderContext context, IServiceCollection services) { }
    protected virtual void Options(ThargaWpfOptions thargaWpfOptions) { }

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

    public static T GetService<T>()
    {
        var service = ((ApplicationBase)Current).AppHost.Services.GetService<T>();
        if (service == null) throw new InvalidOperationException($"Cannot find service '{typeof(T).Name}'. Perhaps it has not been registered in the IOC.");
        return service;
    }

    public static bool Close(CloseMode closeMode = CloseMode.Default)
    {
        try
        {
            CloseMode = closeMode;

            var beforeCloseEventArgs = new BeforeCloseEventArgs();
            BeforeCloseEvent?.Invoke(null, beforeCloseEventArgs);
            if (beforeCloseEventArgs.Cancel) return false;

            //TODO: When closing after an application update, it should be the "Force" mode.

            switch (closeMode)
            {
                case CloseMode.Default:
                case CloseMode.Soft:
                    //TODO: Try to close tabs, if it does not work, abort closing of the application.
                    //TODO: Now that the application could be hidden (icontray) when this happens. Is should be made visible when failed to close.
                    Current?.MainWindow?.Close();
                    break;
                case CloseMode.Force:
                    //TODO: Try to close tabs gently, if it does not work, terminate the application anyway.
                    Current?.MainWindow?.Close();
                    //Current.Shutdown();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(closeMode), closeMode, null);
            }
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

    public static CloseMode CloseMode { get; private set; }
}