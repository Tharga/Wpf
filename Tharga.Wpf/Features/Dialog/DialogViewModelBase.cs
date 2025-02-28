using System.Windows.Input;

namespace Tharga.Wpf.Dialog;

public class DialogViewModelBase : ViewModelBase
{
    public DialogViewModelBase()
    {
        OkCommand = new RelayCommand(_ => { RequestClose(true); }, _ => true);
        CancelCommand = new RelayCommand(_ => { RequestClose(false); }, _ => true);
    }

    public event EventHandler<RequestCloseEvent> RequestCloseEvent;

    protected void RequestClose(bool dialogResult)
    {
        RequestCloseEvent?.Invoke(this, new RequestCloseEvent(dialogResult));
    }

    public ICommand OkCommand { get; protected set; }
    public ICommand CancelCommand { get; protected set; }
}