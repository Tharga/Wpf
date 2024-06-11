using System.Windows.Input;

namespace Tharga.Wpf.Dialog;

public class DialogViewModelBase : ViewModelBase
{
    public event EventHandler<RequestCloseEvent> RequestCloseEvent;

    public ICommand OkCommand => new RelayCommand(_ => { RequestCloseEvent?.Invoke(this, new RequestCloseEvent(true)); }, _ => true);
    public ICommand CancelCommand => new RelayCommand(_ => { RequestCloseEvent?.Invoke(this, new RequestCloseEvent(false)); }, _ => true);
}