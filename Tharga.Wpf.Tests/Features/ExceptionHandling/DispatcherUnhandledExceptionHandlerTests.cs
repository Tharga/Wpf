using System.Windows.Threading;
using Moq;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Tests.Features.ExceptionHandling;

public class DispatcherUnhandledExceptionHandlerTests
{
    [Fact]
    public void An_Exception_Whose_Handler_Awaits_Does_Not_Escape_The_Dispatcher()
    {
        var thrown = new InvalidOperationException("Unhandled on the dispatcher.");
        Exception? handled = null;
        Dispatcher? dispatcher = null;
        var exceptionStateService = new Mock<IExceptionStateService>();
        exceptionStateService
            .Setup(x => x.FallbackHandlerInternalAsync(It.IsAny<Exception>()))
            .Returns<Exception>(async exception =>
            {
                await Task.Yield();
                handled = exception;
                dispatcher!.BeginInvokeShutdown(DispatcherPriority.Normal);
            });
        var sut = new DispatcherUnhandledExceptionHandler(() => exceptionStateService.Object);

        StaThread.RunDispatcher(d =>
        {
            dispatcher = d;
            d.UnhandledException += sut.Handle;
            d.BeginInvoke(() => throw thrown);
        });

        Assert.Same(thrown, handled);
    }

    [Fact]
    public void An_Exception_Whose_Handler_Completes_Synchronously_Does_Not_Escape_The_Dispatcher()
    {
        var thrown = new InvalidOperationException("Unhandled on the dispatcher.");
        var exceptionStateService = new Mock<IExceptionStateService>();
        exceptionStateService.Setup(x => x.FallbackHandlerInternalAsync(It.IsAny<Exception>())).Returns(Task.CompletedTask);
        var sut = new DispatcherUnhandledExceptionHandler(() => exceptionStateService.Object);

        StaThread.RunDispatcher(d =>
        {
            d.UnhandledException += sut.Handle;
            d.BeginInvoke(() => throw thrown);
            d.BeginInvokeShutdown(DispatcherPriority.Background);
        });

        exceptionStateService.Verify(x => x.FallbackHandlerInternalAsync(thrown), Times.Once);
    }

    [Fact]
    public void The_Dispatcher_Keeps_Running_While_The_Handler_Awaits()
    {
        var fallbackRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var steps = new List<string>();
        Dispatcher? dispatcher = null;
        var exceptionStateService = new Mock<IExceptionStateService>();
        exceptionStateService
            .Setup(x => x.FallbackHandlerInternalAsync(It.IsAny<Exception>()))
            .Returns(async () =>
            {
                await fallbackRelease.Task;
                steps.Add("handled");
                dispatcher!.BeginInvokeShutdown(DispatcherPriority.Normal);
            });
        var sut = new DispatcherUnhandledExceptionHandler(() => exceptionStateService.Object);

        StaThread.RunDispatcher(d =>
        {
            dispatcher = d;
            d.UnhandledException += sut.Handle;
            d.BeginInvoke(() => throw new InvalidOperationException("Unhandled on the dispatcher."));
            d.BeginInvoke(() =>
            {
                steps.Add("next operation");
                fallbackRelease.SetResult();
            });
        });

        Assert.Equal(["next operation", "handled"], steps);
    }
}
