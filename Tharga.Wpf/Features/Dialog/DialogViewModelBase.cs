using System.Windows.Input;

namespace Tharga.Wpf.Dialog;

/// <summary>
/// Base class for dialog view models providing Ok/Cancel commands and a close request mechanism.
/// </summary>
public class DialogViewModelBase : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogViewModelBase"/> class with default Ok and Cancel commands.
    /// </summary>
    public DialogViewModelBase()
    {
        OkCommand = new RelayCommand(_ => { RequestClose(true); }, _ => true);
        CancelCommand = new RelayCommand(_ => { RequestClose(false); }, _ => true);
    }

    /// <summary>Raised when the dialog requests to be closed.</summary>
    public event EventHandler<RequestCloseEvent> RequestCloseEvent;

    /// <summary>
    /// Requests the dialog to close with the specified result.
    /// </summary>
    /// <param name="dialogResult">The dialog result (<c>true</c> for Ok, <c>false</c> for Cancel).</param>
    protected void RequestClose(bool dialogResult)
    {
        RequestCloseEvent?.Invoke(this, new RequestCloseEvent(dialogResult));
    }

    /// <summary>Gets the command that closes the dialog with a positive result.</summary>
    public ICommand OkCommand { get; protected set; }

    /// <summary>Gets the command that closes the dialog with a negative result.</summary>
    public ICommand CancelCommand { get; protected set; }
}