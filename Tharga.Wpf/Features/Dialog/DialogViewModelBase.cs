using System.Windows.Input;

namespace Tharga.Wpf.Dialog;

public class DialogViewModelBase : ViewModelBase
{
    public DialogViewModelBase()
    {
        OkCommand = new RelayCommand(_ => { RequestCloseEvent?.Invoke(this, new RequestCloseEvent(true)); }, _ => true);
        CancelCommand = new RelayCommand(_ => { RequestCloseEvent?.Invoke(this, new RequestCloseEvent(false)); }, _ => true);
    }

    public event EventHandler<RequestCloseEvent> RequestCloseEvent;

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }
}