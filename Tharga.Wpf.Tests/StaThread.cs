using System.Runtime.ExceptionServices;
using System.Windows.Threading;

namespace Tharga.Wpf.Tests;

/// <summary>
/// Runs test code on its own STA thread, as WPF objects and dispatchers require.
/// The thread is a background thread and is waited for with a timeout, so a hang fails the test instead of blocking the run.
/// </summary>
internal static class StaThread
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static T Run<T>(Func<T> func, TimeSpan? timeout = null)
    {
        var result = default(T);
        ExceptionDispatchInfo? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                result = func();
            }
            catch (Exception e)
            {
                error = ExceptionDispatchInfo.Capture(e);
            }
            finally
            {
                var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                if (dispatcher is { HasShutdownStarted: false }) dispatcher.InvokeShutdown();
            }
        })
        {
            IsBackground = true
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        if (!thread.Join(timeout ?? DefaultTimeout)) throw new TimeoutException("The STA thread did not finish in time.");
        error?.Throw();
        return result!;
    }

    public static void Run(Action action, TimeSpan? timeout = null)
    {
        Run<object?>(() =>
        {
            action();
            return null;
        }, timeout);
    }

    /// <summary>
    /// Runs a dispatcher loop until the test code shuts it down. An exception that escapes the loop is rethrown to the test.
    /// </summary>
    public static void RunDispatcher(Action<Dispatcher> start, TimeSpan? timeout = null)
    {
        Run(() =>
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(() => start(dispatcher));
            Dispatcher.Run();
        }, timeout);
    }
}
