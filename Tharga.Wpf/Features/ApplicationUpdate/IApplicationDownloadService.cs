namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Retrieves the download location for application updates.
/// </summary>
public interface IApplicationDownloadService
{
    /// <summary>
    /// Gets the application update download location and its source.
    /// </summary>
    /// <returns>A tuple containing the application location URL and its source.</returns>
    Task<(string ApplicationLocation, string ApplicationLocationSource)> GetApplicationLocationAsync();
}