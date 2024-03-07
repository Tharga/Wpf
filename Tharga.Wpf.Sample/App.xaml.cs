using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Sample;

public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        //TODO: AAA: Auto register view models
        services.AddTransient<MainWindowModel>();

        services.AddTransient<MyService>();
    }
}

public class MyService
{
}