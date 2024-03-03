using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System.Reflection;
using Quilt4Net.Ioc;
using Tharga.Wpf.Framework.Exception;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf;

public static class ThargaWpfExtensions
{
    public static void RegisterServiceProvider(this IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<ServiceControl>(ServiceControl.GetServiceControl);
        containerRegistry.RegisterSingleton<IHttpClientFactory>(provider =>
        {
            var serviceControl = provider.Resolve<ServiceControl>();
            var httpClientFactory = serviceControl.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            return httpClientFactory;
        });
        containerRegistry.RegisterSingleton<IConfiguration>(provider =>
        {
            var serviceControl = provider.Resolve<ServiceControl>();
            var configuration = serviceControl.ServiceProvider.GetRequiredService<IConfiguration>();
            return configuration;
        });

        containerRegistry.RegisterSingleton<IExceptionStateService, ExceptionStateService>();

        containerRegistry.RegisterSingleton<ILoggerProvider>(c =>
        {
            var ioc = c.Resolve<IIocProxy>();
            var provider = ioc.GetService<ILoggerProvider>();
            //ioc.StartQuilt4NetEngine();
            return provider;
        });

        containerRegistry.Register(typeof(ILogger), x =>
        {
            var provider = x.Resolve<ILoggerProvider>();
            var categoryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
            var logger = provider.CreateLogger(categoryName);
            return logger;
        });
    }
}