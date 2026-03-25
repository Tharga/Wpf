namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Event arguments containing update information messages.
/// </summary>
public class UpdateInfoEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInfoEventArgs"/> class.
    /// </summary>
    /// <param name="message">The update information message.</param>
    public UpdateInfoEventArgs(string message)
    {
        Message = message;
    }

    /// <summary>Gets the update information message.</summary>
    public string Message { get; }
}