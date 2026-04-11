namespace Tharga.Wpf.Framework;

/// <summary>
/// The resolved action to take when closing.
/// </summary>
public enum CloseAction
{
    /// <summary>Hide the window to the tray.</summary>
    Hide,

    /// <summary>Close tabs softly, then exit.</summary>
    CloseSoft,

    /// <summary>Close tabs forcefully, then exit.</summary>
    CloseForce
}

/// <summary>
/// Resolves the action to take for a given close mode and configuration.
/// Pure logic with no UI dependencies — designed for testability.
/// </summary>
public static class CloseActionResolver
{
    /// <summary>
    /// Determines what action to take based on the close mode and whether hide-on-close is enabled.
    /// </summary>
    /// <param name="closeMode">The close mode requested.</param>
    /// <param name="hideOnClose">Whether the application is configured to hide on default close.</param>
    /// <returns>The action to take.</returns>
    public static CloseAction Resolve(CloseMode closeMode, bool hideOnClose)
    {
        return closeMode switch
        {
            CloseMode.Default when hideOnClose => CloseAction.Hide,
            CloseMode.Default => CloseAction.CloseSoft,
            CloseMode.Soft => CloseAction.CloseSoft,
            CloseMode.Force => CloseAction.CloseForce,
            _ => throw new ArgumentOutOfRangeException(nameof(closeMode), closeMode, null)
        };
    }
}
