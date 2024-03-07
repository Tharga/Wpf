using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Sample;

public partial class App
{
    public App()
    {
        AppHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<MyService>();
            })
            .Build();
    }

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
        var service = ((App)Current).AppHost.Services.GetService<T>();
        if (service == null) throw new InvalidOperationException($"Cannot find service '{typeof(T).Name}'. Perhaps it has not been registered in the IOC.");
        return service;
    }
}

public class MyService
{
}