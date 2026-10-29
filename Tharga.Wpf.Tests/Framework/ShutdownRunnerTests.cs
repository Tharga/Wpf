using System.Windows.Threading;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class ShutdownRunnerTests
{
    private const string Stopped = "stopped";
    private const string Exit = "exit";
    private const string Returned = "returned";
    private const string Error = "error";

    [Fact]
    public void Shutdown_Work_That_Awaits_On_The_Dispatcher_Finishes_Before_Run_Returns()
    {
        var steps = new Steps();

        RunAsOnExit(steps, async _ =>
        {
            await Task.Yield();
            steps.Record(Stopped);
        });

        Assert.Equal([Stopped, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void Shutdown_Work_That_Continues_On_Another_Thread_Finishes_Before_Run_Returns()
    {
        var steps = new Steps();

        RunAsOnExit(steps, async _ =>
        {
            await Task.Delay(20).ConfigureAwait(false);
            steps.Record(Stopped);
        });

        Assert.Equal([Stopped, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void Shutdown_Work_That_Completes_Synchronously_Finishes_Before_Run_Returns()
    {
        var steps = new Steps();

        RunAsOnExit(steps, _ =>
        {
            steps.Record(Stopped);
            return Task.CompletedTask;
        });

        Assert.Equal([Stopped, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void Shutdown_Work_Can_Call_Back_Onto_The_Dispatcher_Without_Deadlocking()
    {
        var steps = new Steps();

        RunAsOnExit(steps, dispatcher => Task.Run(() => dispatcher.Invoke(() => steps.Record(Stopped))));

        Assert.Equal([Stopped, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void Exit_Still_Runs_When_The_Shutdown_Work_Fails_After_An_Await()
    {
        var steps = new Steps();

        RunAsOnExit(steps, async _ =>
        {
            await Task.Yield();
            throw new InvalidOperationException("Stop failed.");
        });

        Assert.Equal([Error, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void Exit_Still_Runs_When_The_Shutdown_Work_Fails_Synchronously()
    {
        var steps = new Steps();

        RunAsOnExit(steps, _ => throw new InvalidOperationException("Stop failed."));

        Assert.Equal([Error, Exit, Returned], steps.ToArray());
    }

    [Fact]
    public void An_Exception_From_Exit_Is_Reported()
    {
        var steps = new Steps();

        RunAsOnExit(steps, _ => Task.CompletedTask, exit: () => throw new InvalidOperationException("Exit handler failed."));

        Assert.Equal([Error, Returned], steps.ToArray());
    }

    /// <summary>
    /// Calls <see cref="ShutdownRunner.Run"/> from a dispatcher operation, then shuts the dispatcher down, as WPF does when OnExit returns.
    /// </summary>
    private static void RunAsOnExit(Steps steps, Func<Dispatcher, Task> shutdownAsync, Action? exit = null)
    {
        StaThread.RunDispatcher(dispatcher =>
        {
            ShutdownRunner.Run(() => shutdownAsync(dispatcher), exit ?? (() => steps.Record(Exit)), _ => steps.Record(Error));
            steps.Record(Returned);
            dispatcher.InvokeShutdown();
        });
    }

    private sealed class Steps
    {
        private readonly List<string> _steps = new();

        public void Record(string step)
        {
            lock (_steps)
            {
                _steps.Add(step);
            }
        }

        public string[] ToArray()
        {
            lock (_steps)
            {
                return _steps.ToArray();
            }
        }
    }
}
