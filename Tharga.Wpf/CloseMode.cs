namespace Tharga.Wpf;

/// <summary>
/// Specifies how the application should handle a close request.
/// </summary>
public enum CloseMode
{
    /// <summary>The default close behaviour.</summary>
    Default,

    /// <summary>Attempt a soft close, allowing handlers to cancel.</summary>
    Soft,

    /// <summary>Force the application to close immediately.</summary>
    Force
}