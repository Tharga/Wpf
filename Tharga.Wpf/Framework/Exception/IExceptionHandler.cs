using System.Windows;

namespace Tharga.Wpf.Framework.Exception;

public interface IExceptionHandler<in T>
    where T : System.Exception
{
    public void Handle(Window mainWindow, T exception);
}