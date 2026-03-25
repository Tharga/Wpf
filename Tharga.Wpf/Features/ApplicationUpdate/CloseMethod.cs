namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Specifies how the splash screen was closed.
/// </summary>
public enum CloseMethod
{
    /// <summary>The splash screen was not closed.</summary>
    None,

    /// <summary>The splash screen was closed automatically after startup completed.</summary>
    Automatically,

    /// <summary>The splash screen was closed manually by the user.</summary>
    Manually
}