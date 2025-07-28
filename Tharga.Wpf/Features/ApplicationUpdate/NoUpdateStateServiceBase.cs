using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal class NoUpdateStateServiceBase : ApplicationUpdateStateServiceBase
{
    public NoUpdateStateServiceBase(IConfiguration configuration, ILoggerFactory loggerFactory, ILicenseClient licenseClient, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, licenseClient, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
    }

    protected override Task UpdateAsync(string clientLocation)
    {
        return Task.CompletedTask;
    }
}