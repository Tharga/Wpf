using System.Windows;

namespace Tharga.Wpf.Features.ExceptionHandling;

public interface IExceptionHandler<in T>
    where T : Exception
{
    public void Handle(Window mainWindow, T exception);
}