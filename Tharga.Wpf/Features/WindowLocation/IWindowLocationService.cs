using System.Windows;

namespace Tharga.Wpf.WindowLocation;

public interface IWindowLocationService
{
    MinitorInfo Monitor(Window window, string name = default, string environment = default);
    void SetVisibility(string name, Visibility visibility);
    string GetFolder(string environment);
}