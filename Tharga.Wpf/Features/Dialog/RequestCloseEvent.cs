namespace Tharga.Wpf.Dialog;

public class RequestCloseEvent : EventArgs
{
    public RequestCloseEvent(bool dialogResult)
    {
        DialogResult = dialogResult;
    }

    public bool? DialogResult { get; }
}