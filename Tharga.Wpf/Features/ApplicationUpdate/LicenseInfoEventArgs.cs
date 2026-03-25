namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Event arguments containing license validation information.
/// </summary>
public class LicenseInfoEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseInfoEventArgs"/> class.
    /// </summary>
    /// <param name="valid">Whether the license is valid.</param>
    /// <param name="message">A message describing the license status.</param>
    public LicenseInfoEventArgs(bool valid, string message)
    {
        Valid = valid;
        Message = message;
    }

    /// <summary>Gets a value indicating whether the license is valid.</summary>
    public bool Valid { get; }

    /// <summary>Gets the license status message.</summary>
    public string Message { get; }
}