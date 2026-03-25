using System.Windows.Input;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.TabNavigator;

/// <summary>
/// An <see cref="ICommand"/> that opens a tab of the specified type when executed.
/// </summary>
/// <typeparam name="TTabView">The type of tab view to open.</typeparam>
public class OpenTabComamnd<TTabView> : ICommand
    where TTabView : TabView
{
    private readonly ITabNavigationStateService _tabNavigationService;
    private readonly Func<bool> _canExecute;
    private readonly Action _postAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTabComamnd{TTabView}"/> class.
    /// </summary>
    /// <param name="tabNavigationService">The tab navigation service used to open tabs.</param>
    /// <param name="canExecute">An optional predicate controlling whether the command can execute.</param>
    /// <param name="postAction">An optional action to run after the tab is opened.</param>
    public OpenTabComamnd(ITabNavigationStateService tabNavigationService, Func<bool> canExecute = default, Action postAction = default)
    {
        _tabNavigationService = tabNavigationService;
        _canExecute = canExecute;
        _postAction = postAction;
    }

    /// <inheritdoc />
    public bool CanExecute(object parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

/// <summary>
/// Preset time filters for document queries.
/// </summary>
public enum EDocumentPreset
{
    /// <summary>Today's documents.</summary>
    Today,
    /// <summary>Yesterday's documents.</summary>
    Yesterday,
    /// <summary>This week's documents.</summary>
    Week,
    /// <summary>Paused documents.</summary>
    Paused
}