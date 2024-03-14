using System.Windows.Input;

//using Tharga.Florida.Client.Framework.Authentication;

namespace Tharga.Wpf.TabNavigator;

public class OpenTabComamnd<TTabView> : ICommand
    where TTabView : TabView
{
    private readonly ITabNavigationStateService _tabNavigationService;
    //private readonly IAuthenticationStateService _authenticationStateService;

    public OpenTabComamnd(ITabNavigationStateService tabNavigationService /*, IAuthenticationStateService authenticationStateService*/)
    {
        _tabNavigationService = tabNavigationService;
        //_authenticationStateService = authenticationStateService;
    }

    public bool CanExecute(object parameter)
    {
        //return _authenticationStateService.IsLoggedIn;
        return true;
    }

    public void Execute(object parameter)
    {
        _tabNavigationService.OpenTab<TTabView>();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}