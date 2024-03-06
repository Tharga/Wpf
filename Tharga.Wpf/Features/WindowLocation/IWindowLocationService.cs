using System.ComponentModel;
using System.Windows;

namespace Tharga.Wpf.Features.WindowLocation;

public interface IWindowLocationService
{
    void Monitor(string name, Window window);
    void AttachProperty(string name, INotifyPropertyChanged container, string propertyName);
}