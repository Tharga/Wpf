//using System.IO;
//using System.Net.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace Tharga.Wpf.Framework;

//public class ServiceControl
//{
//    public ServiceProvider ServiceProvider { get; }
//    public IEnumerable<object> Services => ServiceProvider.GetServices<object>();

//    private ServiceControl(ServiceProvider serviceProvider)
//    {
//        ServiceProvider = serviceProvider;
//    }

//    public static ServiceControl GetServiceControl()
//    {
//        IServiceCollection services = new ServiceCollection();

//        services.AddSingleton<IConfiguration>(_ =>
//        {
//            var builder = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json");
//            return builder.Build();
//        });

//        //TODO: Make this custom, since the config section might differ for different projects
//        services.AddHttpClient<HttpClient>("Api", (serviceProvider, httpClient) =>
//        {
//            var configuration = serviceProvider.GetService<IConfiguration>();
//            httpClient.BaseAddress = Uri.TryCreate(configuration.GetSection("ApiUri").Value, UriKind.Absolute, out var apiAdr) ? apiAdr : throw new InvalidOperationException();
//        });

//        var serviceProvider = services.BuildServiceProvider();
//        return new ServiceControl(serviceProvider);
//    }
//}