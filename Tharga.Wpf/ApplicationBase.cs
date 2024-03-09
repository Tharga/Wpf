using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Features.TabNavigator;
using Tharga.Wpf.Features.WindowLocation;
using Tharga.Wpf.Framework;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf;

public abstract class ApplicationBase : Application
{
    protected ApplicationBase()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            var ess = GetService<IExceptionStateService>();
            ess.FallbackHandlerInternal(e.Exception);
            e.Handled = true;
        };

        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        var options = new ThargaWpfOptions
        {
            ApplicationFullName = assemblyName?.Name ?? "Unknown Application",
            ApplicationShortName = assemblyName?.Name ?? "Unknown",
        };

        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                Options(options);

                services.AddSingleton(_ => options);
                services.AddHttpClient();

                RegisterExceptionHandler(options, services);
                RegisterTabNavigation(services);

                services.AddSingleton<IWindowLocationService>(s =>
                {
                    var logger = s.GetService<ILogger<WindowLocationService>>();
                    return new WindowLocationService(options, logger);
                });

                services.AddSingleton<IApplicationUpdateStateService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var applicationDownloadService = s.GetService<IApplicationDownloadService>();
                    var mainWindow = ((ApplicationBase)Current).MainWindow;
                    var logger = s.GetService<ILogger<ApplicationUpdateStateService>>();
                    return new ApplicationUpdateStateService(configuration, applicationDownloadService, options, mainWindow, logger);
                });
                services.AddTransient<IApplicationDownloadService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var httpClientFactory = s.GetService<IHttpClientFactory>();
                    return new ApplicationDownloadService(configuration, httpClientFactory, options);
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

    private static void RegisterTabNavigation(IServiceCollection services)
    {
        services.AddTransient<TabNavigatorViewModel>();
        services.AddSingleton<ITabNavigationStateService, TabNavigationStateService>();
        var tabViewTypes = TypeHelper.GetTypesBasedOn<TabView>().ToArray();
        foreach (var tabViewTab in tabViewTypes)
        {
            services.AddTransient(tabViewTab);
        }
    }

    protected virtual void Register(HostBuilderContext context, IServiceCollection services) { }
    protected virtual void Options(ThargaWpfOptions thargaWpfOptions) { }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
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
}