using System.ComponentModel;
using System.Windows;

namespace Tharga.Wpf.WindowLocation;

public interface IWindowLocationService
{
    MinitorInfo Monitor(Window window, string name = default, string environment = default);
    void AttachProperty(string name, INotifyPropertyChanged container, string propertyName);
    void SetVisibility(string name, Visibility visibility);
}