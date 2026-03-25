namespace Tharga.Wpf.Dialog;

/// <summary>
/// Event arguments for a dialog close request, carrying the dialog result.
/// </summary>
public class RequestCloseEvent : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestCloseEvent"/> class.
    /// </summary>
    /// <param name="dialogResult">The dialog result.</param>
    public RequestCloseEvent(bool dialogResult)
    {
        DialogResult = dialogResult;
    }

    /// <summary>Gets the dialog result (<c>true</c> for Ok, <c>false</c> for Cancel).</summary>
    public bool? DialogResult { get; }
}