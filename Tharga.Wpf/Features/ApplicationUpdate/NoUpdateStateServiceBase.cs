using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.ApplicationUpdate;

internal class NoUpdateStateServiceBase : ApplicationUpdateStateServiceBase
{
    public NoUpdateStateServiceBase(IConfiguration configuration, ILoggerFactory loggerFactory, IApplicationDownloadService applicationDownloadService, ITabNavigationStateService tabNavigationStateService, ThargaWpfOptions options, Window mainWindow)
        : base(configuration, loggerFactory, applicationDownloadService, tabNavigationStateService, options, mainWindow)
    {
    }

    protected override Task UpdateAsync(string clientLocation)
    {
        return Task.CompletedTask;
    }
}