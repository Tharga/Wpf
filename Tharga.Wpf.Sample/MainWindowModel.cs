using System.Windows;
using System.Windows.Input;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Features.TabNavigator;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Sample;

public class MainWindowModel
{
    private readonly IApplicationUpdateStateService _applicationUpdateStateService;
    private readonly ITabNavigationStateService _tabNavigationStateService;

    public MainWindowModel(MyService myService, IApplicationUpdateStateService applicationUpdateStateService, ITabNavigationStateService tabNavigationStateService)
    {
        _applicationUpdateStateService = applicationUpdateStateService;
        _tabNavigationStateService = tabNavigationStateService;
    }

    public ICommand ShowSplashCommand => new RelayCommand(_ => { _applicationUpdateStateService.ShowSplash(); }, _ => true);
    public ICommand ThrowExceptionCommand => new RelayCommand(_ => throw new InvalidOperationException("Some error."), _ => true);
    public ICommand NewTabCommand => new OpenTabComamnd<MyTabView>(_tabNavigationStateService /*, _authenticationStateService*/);
    public ICommand ExitCommand => new RelayCommand(_ => { Application.Current?.MainWindow?.Close(); }, _ => true);
}