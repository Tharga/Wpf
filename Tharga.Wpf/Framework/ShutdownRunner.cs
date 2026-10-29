using System.Windows.Threading;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Runs an application's asynchronous shutdown work to completion before <c>OnExit</c> returns.
/// WPF shuts the dispatcher down as soon as <c>OnExit</c> returns, which drops any continuation still queued on it, so the work cannot simply be awaited.
/// While the work runs, the dispatcher keeps processing in a nested frame, so continuations and calls marshalled to the UI thread still complete.
/// </summary>
internal static class ShutdownRunner
{
    /// <param name="shutdownAsync">The shutdown work.</param>
    /// <param name="exit">Runs after the shutdown work, whether or not it failed.</param>
    /// <param name="onError">Receives an exception from the shutdown work or from <paramref name="exit"/>.</param>
    public static void Run(Func<Task> shutdownAsync, Action exit, Action<Exception> onError)
    {
        Try(() => WaitWhileProcessingDispatcher(shutdownAsync()), onError);
        Try(exit, onError);
    }

    private static void WaitWhileProcessingDispatcher(Task task)
    {
        if (!task.IsCompleted)
        {
            var frame = new DispatcherFrame();
            task.ContinueWith(_ => frame.Continue = false, TaskScheduler.Default);
            Dispatcher.PushFrame(frame);
        }

        // The frame also ends when the dispatcher itself is shutting down; blocking then would deadlock, so only a finished task is observed.
        if (task.IsCompleted) task.GetAwaiter().GetResult();
    }

    private static void Try(Action action, Action<Exception> onError)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            onError(e);
        }
    }
}
