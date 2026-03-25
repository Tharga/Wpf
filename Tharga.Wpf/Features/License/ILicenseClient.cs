namespace Tharga.Wpf.License;

/// <summary>
/// Client for checking application license validity against a license server.
/// </summary>
public interface ILicenseClient
{
    /// <summary>
    /// Checks whether the application has a valid license.
    /// </summary>
    /// <returns>A tuple indicating whether the license is valid and a descriptive message.</returns>
    Task<(bool IsValid, string Message)> CheckLicenseAsync();
}