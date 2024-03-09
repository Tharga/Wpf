using System.ComponentModel;
using System.Windows;

namespace Tharga.Wpf.WindowLocation;

public interface IWindowLocationService
{
    void Monitor(Window window, string name = default);
    void AttachProperty(string name, INotifyPropertyChanged container, string propertyName);
}