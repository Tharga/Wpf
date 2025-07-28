namespace Tharga.Wpf.License;

public interface ILicenseClient
{
    Task<(bool IsValid, string Message)> CheckLicenseAsync();
}