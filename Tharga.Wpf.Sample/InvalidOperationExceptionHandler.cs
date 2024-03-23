using System.Windows;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Sample;

public class InvalidOperationExceptionHandler : IExceptionHandler<InvalidOperationException>
{
    public void Handle(Window mainWindow, InvalidOperationException exception)
    {
        MessageBox.Show(mainWindow, $"This is a custom error handler for '{exception.Message}'.", exception.GetType().Name);
    }
}