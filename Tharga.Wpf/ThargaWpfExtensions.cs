using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System.Reflection;
using Quilt4Net.Ioc;
using Tharga.Wpf.Framework.Exception;
using Tharga.Wpf.Framework;
using Tharga.Wpf.Features.ApplicationUpdate;

namespace Tharga.Wpf;

public class ThargaWpfOptions
{
    private readonly ConcurrentDictionary<Type, Type> _exceptionTypes = new();

    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    public IDictionary<Type, Type> GetExceptionTypes()
    {
        return _exceptionTypes;
    }
}

public static class ThargaWpfExtensions
{
    public static void RegisterServiceProvider(this IContainerRegistry containerRegistry, Action<ThargaWpfOptions> options = default)
    {
        var o = new ThargaWpfOptions();
        options?.Invoke(o);

        containerRegistry.RegisterSingleton<IApplicationUpdateStateService, ApplicationUpdateStateService>();
        containerRegistry.RegisterSingleton<IWindowLocationService, WindowLocationService>();
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

        foreach (var exceptionType in o.GetExceptionTypes())
        {
            containerRegistry.Register(exceptionType.Value);
        }

        containerRegistry.RegisterSingleton<IExceptionStateService>(c =>
        {
            var logger = c.Resolve < ILogger>();
            return new ExceptionStateService(c, logger, o.GetExceptionTypes());
        });

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