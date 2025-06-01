using System.Windows.Input;
using Tharga.Wpf.Framework;

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

    public async void Execute(object parameter)
    {
        try
        {
            await _tabNavigationService.OpenTabAsync<TTabView>(parameter: parameter);
            _postAction?.Invoke();
        }
        catch (Exception e)
        {
            StaticExceptionHandler.Handle(e, this);
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public enum EDocumentPreset
{
    Today,
    Yesterday,
    Week,
    Paused
}