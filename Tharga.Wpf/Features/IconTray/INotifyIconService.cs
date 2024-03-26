using System.Windows;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.IconTray;

public interface INotifyIconService
{
    void Create(Window window, string name, IWindowLocationService windowLocationService);
}