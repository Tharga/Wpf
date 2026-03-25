namespace Tharga.Wpf;

/// <summary>
/// Event arguments for the <see cref="ApplicationBase.BeforeCloseEvent"/>, allowing handlers to cancel the close operation.
/// </summary>
public class BeforeCloseEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the close operation should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
}