using System.Windows.Media;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Sample;

public partial class App
{
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
        thargaWpfOptions.CompanyName = "Tharga";
        thargaWpfOptions.Debug = false;

        thargaWpfOptions.HideOnClose = true;
        //thargaWpfOptions.StartupWindowState = StartupWindowState.Last;

        // Demo the dark splash image + light foreground brush (so the overlaid text stays readable).
        thargaWpfOptions.SplashCreator = data => new Splash(data with { ImagePath = SplashImageLibrary.Colours });
        thargaWpfOptions.SplashForeground = Brushes.White;

        thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();

        thargaWpfOptions.UpdateLocation += _ => "https://app-eplicta-aggregator-ci.azurewebsites.net/api/Agent";
    }
}