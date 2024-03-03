using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System.Reflection;
using Quilt4Net.Ioc;
using Tharga.Wpf.Framework.Exception;
using Tharga.Wpf.Framework;
using Tharga.Wpf.Features.ApplicationUpdate;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf;

public static class ThargaWpfExtensions
{
    public static void RegisterServiceProvider(this IContainerRegistry containerRegistry, Action<ThargaWpfOptions> options = default)
    {
        var o = new ThargaWpfOptions
        {
            ApplicationFullName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationFullName)} to set the full name of the application."),
            ApplicationShortName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationShortName)} to set the short name of the application.")
        };
        options?.Invoke(o);

        containerRegistry.RegisterSingleton<IApplicationUpdateStateService>(c =>
        {
            var configuration = c.Resolve<IConfiguration>();
            var logger = c.Resolve<ILogger>();
            return new ApplicationUpdateStateService(configuration, o, logger);
        });
        containerRegistry.RegisterSingleton<IWindowLocationService, WindowLocationService>();

        //TODO: The ServiceControl could possibly be change now
        //IServiceCollection services = new ServiceCollection();

        //foreach (var registration in registrations)
        //{
        //    registration.Invoke(services);
        //}

        //containerRegistry.RegisterSingleton<IConfiguration >(_ =>
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json");
        //    return builder.Build();
        //});

        ////TODO: Make this custom, since the config section might differ for different projects
        //serviceCollection.AddHttpClient<HttpClient>("Api", (serviceProvider, httpClient) =>
        //{
        //    var configuration = serviceProvider.GetService<IConfiguration>();
        //    httpClient.BaseAddress = Uri.TryCreate(configuration.GetSection("ApiUri").Value, UriKind.Absolute, out var apiAdr) ? apiAdr : throw new InvalidOperationException();
        //});

        //var serviceProvider = services.BuildServiceProvider();
        //return new ServiceControl(serviceProvider);

        var serviceConstrol = ServiceControl.GetServiceControl();
        containerRegistry.RegisterSingleton<ServiceControl>(c => serviceConstrol);

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
            //serviceCollection.AddTransient(exceptionType.Value);
        }

        containerRegistry.RegisterSingleton<IExceptionStateService>(c =>
        {
            var logger = c.Resolve<ILogger>();
            return new ExceptionStateService(c, logger, o.GetExceptionTypes());
        });

        containerRegistry.RegisterSingleton<ILoggerProvider>(c =>
        {
            var ioc = c.Resolve<IIocProxy>();
            var provider = ioc.GetService<ILoggerProvider>();
            //ioc.StartQuilt4NetEngine();
            return provider;
        });

        containerRegistry.RegisterSingleton(typeof(ILogger), x =>
        {
            var provider = x.Resolve<ILoggerProvider>();
            var categoryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
            var logger = provider.CreateLogger(categoryName);
            return logger;
        });

        //containerRegistry.RegisterSingleton<IConfiguration>(c =>
        //{
        //    var p = c.Resolve<IServiceProvider>();
        //    throw new NotImplementedException();
        //});

        containerRegistry.RegisterSingleton<IConfiguration>(_ =>
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return builder.Build();
        });
    }

    //public static void RegisterServiceProvider(this IContainerRegistry containerRegistry, Action<ThargaWpfOptions> options = default)
    //{
    //    var o = new ThargaWpfOptions
    //    {
    //        ApplicationFullName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationFullName)} to set the full name of the application."),
    //        ApplicationShortName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException($"Cannot find name from entry assembly. Provide the option {nameof(ThargaWpfOptions.ApplicationShortName)} to set the short name of the application.")
    //    };
    //    options?.Invoke(o);

    //    containerRegistry.RegisterSingleton<IApplicationUpdateStateService>(c =>
    //    {
    //        var configuration = c.Resolve<IConfiguration>();
    //        var logger = c.Resolve<ILogger>();
    //        return new ApplicationUpdateStateService(configuration, o, logger);
    //    });
    //    containerRegistry.RegisterSingleton<IWindowLocationService, WindowLocationService>();

    //    //TODO: The ServiceControl could possibly be change now
    //    var serviceConstrol = ServiceControl.GetServiceControl(o.GetServiceRegistrations());
    //    containerRegistry.RegisterSingleton<ServiceControl>(c => serviceConstrol);

    //    containerRegistry.RegisterSingleton<IHttpClientFactory>(provider =>
    //    {
    //        var serviceControl = provider.Resolve<ServiceControl>();
    //        var httpClientFactory = serviceControl.ServiceProvider.GetRequiredService<IHttpClientFactory>();
    //        return httpClientFactory;
    //    });
    //    containerRegistry.RegisterSingleton<IConfiguration>(provider =>
    //    {
    //        var serviceControl = provider.Resolve<ServiceControl>();
    //        var configuration = serviceControl.ServiceProvider.GetRequiredService<IConfiguration>();
    //        return configuration;
    //    });

    //    foreach (var exceptionType in o.GetExceptionTypes())
    //    {
    //        containerRegistry.Register(exceptionType.Value);
    //    }

    //    containerRegistry.RegisterSingleton<IExceptionStateService>(c =>
    //    {
    //        var logger = c.Resolve<ILogger>();
    //        return new ExceptionStateService(c, logger, o.GetExceptionTypes());
    //    });

    //    containerRegistry.RegisterSingleton<ILoggerProvider>(c =>
    //    {
    //        var ioc = c.Resolve<IIocProxy>();
    //        var provider = ioc.GetService<ILoggerProvider>();
    //        //ioc.StartQuilt4NetEngine();
    //        return provider;
    //    });

    //    containerRegistry.Register(typeof(ILogger), x =>
    //    {
    //        var provider = x.Resolve<ILoggerProvider>();
    //        var categoryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
    //        var logger = provider.CreateLogger(categoryName);
    //        return logger;
    //    });
    //}
}