//using Microsoft.Extensions.DependencyInjection;

//namespace Tharga.Wpf.Framework;

//internal static class ThargaWpfExtensions
//{
//    public static void RegisterServiceProvider(this IServiceCollection services, Action<ThargaWpfOptions> options = default)
//    {
//        //var serviceConstrol = ServiceControl.GetServiceControl();
//        //services.AddSingleton<ServiceControl>(_ => serviceConstrol);

//        //services.AddSingleton<IHttpClientFactory>(provider =>
//        //{
//        //    var serviceControl = provider.GetService<ServiceControl>();
//        //    var httpClientFactory = serviceControl.ServiceProvider.GetRequiredService<IHttpClientFactory>();
//        //    return httpClientFactory;
//        //});
//        //services.AddSingleton<IConfiguration>(provider =>
//        //{
//        //    var serviceControl = provider.GetService<ServiceControl>();
//        //    var configuration = serviceControl.ServiceProvider.GetRequiredService<IConfiguration>();
//        //    return configuration;
//        //});

//        //foreach (var exceptionType in o.GetExceptionTypes())
//        //{
//        //    services.AddTransient(exceptionType.Value);
//        //}

//        //services.AddSingleton<ILoggerProvider>(c =>
//        //{
//        //    var ioc = c.GetService<IIocProxy>();
//        //    var provider = ioc.GetService<ILoggerProvider>();
//        //    //ioc.StartQuilt4NetEngine();
//        //    return provider;
//        //});

//        //services.AddSingleton(typeof(ILogger), x =>
//        //{
//        //    var provider = x.GetService<ILoggerProvider>();
//        //    var categoryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
//        //    var logger = provider.CreateLogger(categoryName);
//        //    return logger;
//        //});

//        //services.AddSingleton<IConfiguration>(_ =>
//        //{
//        //    var builder = new ConfigurationBuilder()
//        //        .SetBasePath(Directory.GetCurrentDirectory())
//        //        .AddJsonFile("appsettings.json");
//        //    return builder.Build();
//        //});
//    }
//}