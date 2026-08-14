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

    [Fact]
    public void MutexName_Is_Machine_Scoped()
    {
        Assert.StartsWith(@"Global\", SingleInstanceService.BuildMutexName("Whatever"));
    }

    [Fact]
    public void MutexName_Is_Not_Session_Scoped()
    {
        var mutexName = SingleInstanceService.BuildMutexName("Whatever");

        // An unprefixed name resolves in Local\, which Windows scopes per Terminal Services
        // session, so two signed-in users would each acquire their own lock.
        Assert.DoesNotContain(@"Local\", mutexName);
        Assert.NotEqual("Tharga.Wpf.Whatever", mutexName);
    }

    [Fact]
    public void PipeName_Stays_Unprefixed()
    {
        // Named pipes are already machine-wide, so the show-signal path must NOT gain the
        // Global\ prefix — prefixing it would break signalling rather than widen it.
        Assert.Equal("Tharga.Wpf.Whatever", SingleInstanceService.BuildPipeName("Whatever"));
    }

    [Fact]
    public void MutexName_And_PipeName_Are_Different()
    {
        // They were one shared field before; only the mutex is namespace-scoped.
        var name = "Whatever";
        Assert.NotEqual(SingleInstanceService.BuildPipeName(name), SingleInstanceService.BuildMutexName(name));
    }

    [Fact]
    public void Instance_Exposes_The_Built_Names()
    {
        var name = TestAppName + nameof(Instance_Exposes_The_Built_Names);
        using var service = new SingleInstanceService(name);

        Assert.Equal(SingleInstanceService.BuildMutexName(name), service.MutexName);
        Assert.Equal(SingleInstanceService.BuildPipeName(name), service.PipeName);
    }

    [Fact]
    public void Acquired_Mutex_Is_Visible_Under_Its_Global_Name()
    {
        var name = TestAppName + nameof(Acquired_Mutex_Is_Visible_Under_Its_Global_Name);
        using var service = new SingleInstanceService(name);
        Assert.True(service.TryAcquire());

        // Opening the same Global\ name from outside the service proves the lock really lives
        // in the machine-wide namespace, not merely that the string starts with a prefix.
        Assert.True(Mutex.TryOpenExisting(SingleInstanceService.BuildMutexName(name), out var existing));
        existing?.Dispose();
    }

    [Fact]
    public void Unprefixed_Name_Does_Not_Collide_With_The_Global_Lock()
    {
        var name = TestAppName + nameof(Unprefixed_Name_Does_Not_Collide_With_The_Global_Lock);
        using var service = new SingleInstanceService(name);
        Assert.True(service.TryAcquire());

        // The old (session-scoped) name must be a genuinely different kernel object — this is
        // the regression that made the guard per-session.
        using var sessionScoped = new Mutex(true, SingleInstanceService.BuildPipeName(name), out var createdNew);
        Assert.True(createdNew);
    }
}
