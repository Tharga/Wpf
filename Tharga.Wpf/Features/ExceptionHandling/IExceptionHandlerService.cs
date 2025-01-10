using System.Windows;

namespace Tharga.Wpf.ExceptionHandling;

public interface IExceptionHandlerService
{
    Task<bool> HandleExceptionAsync(Exception exception, Window mainWindow);
}