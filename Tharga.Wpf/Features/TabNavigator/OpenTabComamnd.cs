using System.Windows.Input;

namespace Tharga.Wpf.TabNavigator;

public class OpenTabComamnd<TTabView> : ICommand
    where TTabView : TabView
{
    private readonly ITabNavigationStateService _tabNavigationService;
    private readonly Func<bool> _canExecute;
    private readonly Action _postAction;

    public OpenTabComamnd(ITabNavigationStateService tabNavigationService, Func<bool> canExecute = default, Action postAction = default)
    {
        _tabNavigationService = tabNavigationService;
        _canExecute = canExecute;
        _postAction = postAction;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke() ?? true;
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