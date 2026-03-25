using System.Windows.Input;

namespace Tharga.Wpf;

/// <summary>
/// A basic <see cref="ICommand"/> implementation that delegates execution to an <see cref="Action{T}"/>.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute when the command is invoked.</param>
    /// <param name="canExecute">An optional predicate that determines whether the command can execute. Defaults to always <c>true</c>.</param>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
    }

    /// <inheritdoc />
    public bool CanExecute(object parameter)
    {
        return _canExecute(parameter);
    }

    /// <inheritdoc />
    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}