using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.ExceptionHandling;
using Tharga.Wpf.Framework;
using Tharga.Wpf.IconTray;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf;

public abstract class ApplicationBase : Application
{
    private readonly ThargaWpfOptions _options;
    private Mutex _mutex;

    protected ApplicationBase()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            var ess = GetService<IExceptionStateService>();
            ess.FallbackHandlerInternal(e.Exception);
            e.Handled = true;
        };

        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
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

                foreach (var viewModel in TypeHelper.GetTypesBasedOn<IViewModel>())
                {
                    services.AddTransient(viewModel);
                }

                services.AddSingleton<IWindowLocationService>(s =>
                {
                    var logger = s.GetService<ILogger<WindowLocationService>>();
                    return new WindowLocationService(_options, logger);
                });

                services.AddSingleton<IApplicationUpdateStateService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var applicationDownloadService = s.GetService<IApplicationDownloadService>();
                    var tabNavigationStateService = s.GetService<ITabNavigationStateService>();
                    var mainWindow = ((ApplicationBase)Current).MainWindow;
                    var logger = s.GetService<ILogger<ApplicationUpdateStateService>>();
                    return new ApplicationUpdateStateService(configuration, applicationDownloadService, tabNavigationStateService, _options, mainWindow, logger);
                });
                services.AddTransient<IApplicationDownloadService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var httpClientFactory = s.GetService<IHttpClientFactory>();
                    return new ApplicationDownloadService(configuration, httpClientFactory, _options);
                });

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
        services.AddSingleton<IExceptionStateService>(s =>
        {
            var logger = s.GetService<ILogger<ExceptionStateService>>();
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
        services.AddSingleton<INotifyIconService>(s => new NotifyIconService(options));
    }

    protected virtual void Register(HostBuilderContext context, IServiceCollection services) { }
    protected virtual void Options(ThargaWpfOptions thargaWpfOptions) { }

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (!_options.AllowMultipleApplications)
        {
            _mutex = new Mutex(true, _options.ApplicationShortName, out var createdNew);
            if (!createdNew)
            {
                BringExistingInstanceToFront();
                Shutdown();
                return;
            }
        }

        await AppHost.StartAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _mutex?.Close();
        await AppHost.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }

    public static T GetService<T>()
    {
        var service = ((ApplicationBase)Current).AppHost.Services.GetService<T>();
        if (service == null) throw new InvalidOperationException($"Cannot find service '{typeof(T).Name}'. Perhaps it has not been registered in the IOC.");
        return service;
    }

    public static void Close(CloseMode closeMode = CloseMode.Default)
    {
        try
        {
            CloseMode = closeMode;

            //TODO: When closing after an application update, it should be the "Force" mode.

            switch (closeMode)
            {
                case CloseMode.Default:
                case CloseMode.Soft:
                    //TODO: Try to close tabs, if it does not work, abort closing of the application.
                    //TODO: Not that the application could be hidden (icontray) when this happens. Is should be made visible when failed to close.
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
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            CloseMode = CloseMode.Default;
        }
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