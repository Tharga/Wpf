using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Features.WindowLocation;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf;

public abstract class ApplicationBase : Application
{
    protected ApplicationBase()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            e.Exception.FallbackHandler(this);
            e.Handled = true;
        };

        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        var thargaWpfOptions = new ThargaWpfOptions
        {
            ApplicationFullName = assemblyName?.Name ?? "Unknown Application",
            ApplicationShortName = assemblyName?.Name ?? "Unknown",
        };

        AppHost = Host
            .CreateDefaultBuilder()
            //TODO: AAA: Load configuration
            //.ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!); })
            .ConfigureServices((context, services) =>
            {
                Options(thargaWpfOptions);
                var exceptionHandlers = thargaWpfOptions.GetExceptionTypes();
                foreach (var exceptionHandler in exceptionHandlers)
                {
                    services.AddTransient(exceptionHandler.Value);
                }

                services.AddSingleton<IApplicationUpdateStateService>(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var logger = s.GetService<ILogger<ApplicationUpdateStateService>>();
                    return new ApplicationUpdateStateService(configuration, thargaWpfOptions, logger);
                });
                services.AddSingleton<IWindowLocationService, WindowLocationService>();
                services.AddSingleton<IExceptionStateService>(s =>
                {
                    var logger = s.GetService<ILogger<ExceptionStateService>>();
                    return new ExceptionStateService(s, logger, exceptionHandlers);
                });
                Register(context, services);
            })
            .Build();
    }

    public IHost AppHost { get; protected init; }

    protected abstract void Register(HostBuilderContext context, IServiceCollection services);

    protected virtual void Options(ThargaWpfOptions thargaWpfOptions)
    {
    }

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