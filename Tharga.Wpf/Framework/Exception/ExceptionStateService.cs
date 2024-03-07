using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace Tharga.Wpf.Framework.Exception;

internal class ExceptionStateService : IExceptionStateService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExceptionStateService> _logger;
    private readonly IDictionary<Type, Type> _exceptionHandlers;
    private readonly Window _mainWindow;

    public ExceptionStateService(IServiceProvider serviceProvider, Window mainWindow, ILogger<ExceptionStateService> logger, IDictionary<Type, Type> exceptionHandlers)
    {
        _serviceProvider = serviceProvider;
        _mainWindow = mainWindow;
        _logger = logger;
        _exceptionHandlers = exceptionHandlers;
    }

    //private static event EventHandler<HandleExceptionEventArgs> HandleExceptionEvent;

    //public void AttachMainWindow(Window mainWindow)
    //{
    //    if (_mainWindow != null) throw new InvalidOperationException("The main window has already been attached.");

    //    HandleExceptionEvent += (_, e) => { FallbackHandlerInternal(e.Exception); };

    //    _mainWindow = mainWindow;
    //}

    //internal static void FallbackHandler(object sender, System.Exception exception)
    //{
    //    HandleExceptionEvent?.Invoke(sender, new HandleExceptionEventArgs(exception));
    //}

    public void FallbackHandlerInternal(System.Exception exception)
    {
        try
        {
            if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handlerType))
            {
                var handler = _serviceProvider.GetService(handlerType) ?? throw new NullReferenceException($"Cannot find error handler type '{handlerType.Name}'.");
                var method = handler.GetType().GetMethod(nameof(IExceptionHandler<System.Exception>.Handle));
                method?.Invoke(handler, [_mainWindow, exception]);
                return;
            }

            var exceptionTypeName = exception.GetType().Name;
            var message = exception.InnerException?.Message.NullIfEmpty() ?? exception.Message.NullIfEmpty() ?? exceptionTypeName;
            if (_mainWindow != null)
            {
                switch (exceptionTypeName)
                {
                    case nameof(NotImplementedException):
                    case nameof(InvalidOperationException):
                    case nameof(NotSupportedException):
                        MessageBox.Show(_mainWindow, message, exceptionTypeName, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        Debugger.Break();
                        MessageBox.Show(_mainWindow, $"{message}\n\n@{exception.StackTrace}", $"Unexpected {exceptionTypeName}.", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else if (Application.Current.MainWindow != null)
            {
                MessageBox.Show(Application.Current.MainWindow, message, "Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(message, "Fatal (No Main Window)", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _logger?.LogError(exception, exception.Message);
        }
        catch (System.Exception e)
        {
            _logger?.LogCritical(e, e.Message);
            Debugger.Break();
            MessageBox.Show($"Error in the error handler. {e.Message}", e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}