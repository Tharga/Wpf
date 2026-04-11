using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Sample;

public partial class App
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        _ = new MainWindow();

        var updateService = GetService<IApplicationUpdateStateService>();
        await updateService.ShowSplashAsync(checkForUpdates: true);
    }

    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
        services.AddTransient<FixedTabView>();
    }

    protected override void Options(ThargaWpfOptions thargaWpfOptions)
    {
        thargaWpfOptions.ApplicationShortName = "Sample";
        thargaWpfOptions.ApplicationFullName = "Tharga Wpf Sample Application";
        thargaWpfOptions.UpdateIntervalCheck = TimeSpan.Zero;
        thargaWpfOptions.AllowTabsWithSameTitles = false;
        thargaWpfOptions.AllowMultipleApplications = false;

        thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();

        thargaWpfOptions.UpdateLocation += _ => "https://app-eplicta-aggregator-ci.azurewebsites.net/api/Agent";
    }
}