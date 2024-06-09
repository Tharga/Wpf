using System.Windows.Input;

namespace Tharga.Wpf.TabNavigator;

public class OpenTabComamnd<TTabView> : ICommand
    where TTabView : TabView
{
    private readonly ITabNavigationStateService _tabNavigationService;
    private readonly Action _postAction;

    public OpenTabComamnd(ITabNavigationStateService tabNavigationService, Action postAction = default)
    {
        _tabNavigationService = tabNavigationService;
        _postAction = postAction;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _tabNavigationService.OpenTab<TTabView>();
        _postAction?.Invoke();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}