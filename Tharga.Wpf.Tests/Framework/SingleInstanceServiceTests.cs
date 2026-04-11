using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class SingleInstanceServiceTests
{
    private const string TestAppName = "TestApp_SingleInstance";

    [Fact]
    public void TryAcquire_FirstInstance_Returns_True()
    {
        using var service = new SingleInstanceService(TestAppName + nameof(TryAcquire_FirstInstance_Returns_True));
        Assert.True(service.TryAcquire());
    }

    [Fact]
    public void TryAcquire_SecondInstance_Returns_False()
    {
        var name = TestAppName + nameof(TryAcquire_SecondInstance_Returns_False);
        using var first = new SingleInstanceService(name);
        Assert.True(first.TryAcquire());

        using var second = new SingleInstanceService(name);
        Assert.False(second.TryAcquire());
    }

    [Fact]
    public void TryAcquire_AfterRelease_Returns_True()
    {
        var name = TestAppName + nameof(TryAcquire_AfterRelease_Returns_True);
        using var first = new SingleInstanceService(name);
        Assert.True(first.TryAcquire());
        first.ReleaseMutex();

        using var second = new SingleInstanceService(name);
        Assert.True(second.TryAcquire());
    }

    [Fact]
    public async Task Signal_Triggers_ShowCallback()
    {
        var name = TestAppName + nameof(Signal_Triggers_ShowCallback);
        using var service = new SingleInstanceService(name);
        service.TryAcquire();

        var showCalled = new TaskCompletionSource<bool>();
        service.StartListening(() => showCalled.TrySetResult(true));

        // Give the listener a moment to start.
        await Task.Delay(100);

        SingleInstanceService.SignalExistingInstance(name);

        var result = await Task.WhenAny(showCalled.Task, Task.Delay(3000));
        Assert.True(showCalled.Task.IsCompleted, "Show callback was not invoked within timeout.");
        Assert.True(showCalled.Task.Result);
    }

    [Fact]
    public void ReleaseMutex_Twice_Does_Not_Throw()
    {
        var name = TestAppName + nameof(ReleaseMutex_Twice_Does_Not_Throw);
        using var service = new SingleInstanceService(name);
        service.TryAcquire();
        service.ReleaseMutex();
        service.ReleaseMutex(); // Should not throw.
    }

    [Fact]
    public void Dispose_Without_Acquire_Does_Not_Throw()
    {
        var name = TestAppName + nameof(Dispose_Without_Acquire_Does_Not_Throw);
        var service = new SingleInstanceService(name);
        service.Dispose(); // Should not throw.
    }
}
