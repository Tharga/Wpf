using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Sample;

public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions thargaWpfOptions)
    {
        thargaWpfOptions.ApplicationShortName = "Sample";
        thargaWpfOptions.ApplicationFullName = "Tharga Wpf Sample Application";
        thargaWpfOptions.CheckForUpdateInterval = TimeSpan.Zero;
        thargaWpfOptions.AllowTabsWithSameTitles = false;

        thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();

        thargaWpfOptions.ApplicationDownloadLocationLoader += _ => { return new Uri("https://app-eplicta-aggregator-ci.azurewebsites.net/api/Agent"); };
    }
}