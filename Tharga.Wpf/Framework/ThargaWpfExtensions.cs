using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Quilt4Net.Ioc;
using Tharga.Wpf.Framework.Exception;
using Tharga.Wpf.Features.ApplicationUpdate;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Framework;

internal static class ThargaWpfExtensions
{
    public static void RegisterServiceProvider(this IServiceCollection services, Action<ThargaWpfOptions> options = default)
    {
        //var o = new ThargaWpfOptions
        //{
        //    ApplicationFullName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationFullName)} to set the full name of the application."),
        //    ApplicationShortName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationShortName)} to set the short name of the application.")
        //};
        //options?.Invoke(o);

        //services.AddSingleton<IApplicationUpdateStateService>(c =>
        //{
        //    var configuration = c.GetService<IConfiguration>();
        //    var logger = c.GetService<ILogger>();
        //    return new ApplicationUpdateStateService(configuration, o, logger);
        //});
        //services.AddSingleton<IWindowLocationService, WindowLocationService>();

        //var serviceConstrol = ServiceControl.GetServiceControl();
        //services.AddSingleton<ServiceControl>(_ => serviceConstrol);

        //services.AddSingleton<IHttpClientFactory>(provider =>
        //{
        //    var serviceControl = provider.GetService<ServiceControl>();
        //    var httpClientFactory = serviceControl.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        //    return httpClientFactory;
        //});
        //services.AddSingleton<IConfiguration>(provider =>
        //{
        //    var serviceControl = provider.GetService<ServiceControl>();
        //    var configuration = serviceControl.ServiceProvider.GetRequiredService<IConfiguration>();
        //    return configuration;
        //});

        //foreach (var exceptionType in o.GetExceptionTypes())
        //{
        //    services.AddTransient(exceptionType.Value);
        //}

        //services.AddSingleton<IExceptionStateService>(c =>
        //{
        //    var logger = c.GetService<ILogger>();
        //    return new ExceptionStateService(c, logger, o.GetExceptionTypes());
        //});

        //services.AddSingleton<ILoggerProvider>(c =>
        //{
        //    var ioc = c.GetService<IIocProxy>();
        //    var provider = ioc.GetService<ILoggerProvider>();
        //    //ioc.StartQuilt4NetEngine();
        //    return provider;
        //});

        //services.AddSingleton(typeof(ILogger), x =>
        //{
        //    var provider = x.GetService<ILoggerProvider>();
        //    var categoryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
        //    var logger = provider.CreateLogger(categoryName);
        //    return logger;
        //});

        //services.AddSingleton<IConfiguration>(_ =>
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json");
        //    return builder.Build();
        //});
    }
}