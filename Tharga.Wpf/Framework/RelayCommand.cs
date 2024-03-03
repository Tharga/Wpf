using System.Windows.Input;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf.Framework;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        try
        {
            _execute(parameter);
        }
        catch (System.Exception exception)
        {
            exception.FallbackHandler(this);
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}