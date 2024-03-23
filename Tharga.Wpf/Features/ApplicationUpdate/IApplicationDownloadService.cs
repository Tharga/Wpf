namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationDownloadService
{
    Task<(string ApplicationLocation, string ApplicationLocationSource)> GetApplicationLocationAsync();
}