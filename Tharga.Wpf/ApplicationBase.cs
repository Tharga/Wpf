﻿using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                RegisterExceptionHandler(options, services);

                //services.AddSingleton<IApplicationUpdateStateService>(s =>
                //{
                //    var configuration = s.GetService<IConfiguration>();
                //    var logger = s.GetService<ILogger<ApplicationUpdateStateService>>();
                //    var something = s.GetService<IApplicationDownloadService>();
                //    return new ApplicationUpdateStateService(configuration, something, options, logger);
                //});
                //services.AddTransient<IApplicationDownloadService>(s =>
                //{
                //    var configuration = s.GetService<IConfiguration>();
                //    var httpClientFactory = s.GetService<IHttpClientFactory>();
                //    return new ApplicationDownloadService(configuration, httpClientFactory, options);
                //});
                //services.AddSingleton<IWindowLocationService, WindowLocationService>();
                Register(context, services);
            })
            .Build();
    }

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

    protected abstract void Register(HostBuilderContext context, IServiceCollection services);

    public IHost AppHost { get; protected init; }

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

    protected virtual void Options(ThargaWpfOptions thargaWpfOptions)
    {
    }
}