using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Sample;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        //TODO: now set the Green color scheme and dark base color
        //ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Green");
        base.OnStartup(e);
    }

    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions thargaWpfOptions)
    {
        thargaWpfOptions.ApplicationShortName = "Sample";
        thargaWpfOptions.ApplicationFullName = "Tharga Wpf Sample Application";

        thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();

        thargaWpfOptions.ApplicationDownloadLocationLoader += (c) =>
        {
            //var baseUri = new Uri(c.GetSection("AggregatorUri").Get<string>());
            //Uri.TryCreate(baseUri, "api/Agent", out var uri);
            //return uri;
            return new Uri("https://app-eplicta-aggregator-ci.azurewebsites.net/api/Agent");
        };
    }
}

public class InvalidOperationExceptionHandler : IExceptionHandler<InvalidOperationException>
{
    public void Handle(Window mainWindow, InvalidOperationException exception)
    {
        MessageBox.Show(mainWindow, $"This is a custom error handler for '{exception.Message}'.", exception.GetType().Name);
    }
}

public class MyService
{
}