using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tharga.Wpf;

public abstract class ApplicationBase : Application
{
    protected ApplicationBase()
    {
        AppHost = Host
            .CreateDefaultBuilder()
            //TODO: AAA: Load configuration
            //.ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!); })
            .ConfigureServices((context, services) =>
            {
                Register(context, services);
            })
            .Build();
    }

    public IHost AppHost { get; protected init; }

    protected abstract void Register(HostBuilderContext context, IServiceCollection services);

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
        return ((ApplicationBase)Current).AppHost.Services.GetService<T>();
    }
}