namespace Tharga.Wpf.ApplicationUpdate;

public interface IApplicationDownloadService
{
    Task<string> GetApplicationLocationAsync();
}