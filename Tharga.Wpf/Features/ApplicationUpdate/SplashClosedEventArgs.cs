namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Event arguments raised when the splash screen completes.
/// </summary>
public class SplashCompleteEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplashCompleteEventArgs"/> class.
    /// </summary>
    /// <param name="closeMethod">How the splash screen was closed.</param>
    /// <param name="closed">Whether the splash screen was actually closed.</param>
    public SplashCompleteEventArgs(CloseMethod closeMethod, bool closed)
    {
        CloseMethod = closeMethod;
        Closed = closed;
    }

    /// <summary>Gets the method by which the splash screen was closed.</summary>
    public CloseMethod CloseMethod { get; }

    /// <summary>Gets a value indicating whether the splash screen was closed.</summary>
    public bool Closed { get; }
}