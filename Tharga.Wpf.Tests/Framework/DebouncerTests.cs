using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class DebouncerTests
{
    [Fact]
    public async Task Action_Runs_Once_After_The_Delay()
    {
        var count = 0;
        using var debouncer = new Debouncer(TimeSpan.FromMilliseconds(50), () => Interlocked.Increment(ref count));

        debouncer.Trigger();
        await Task.Delay(300, TestContext.Current.CancellationToken);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Repeated_Triggers_Collapse_To_One_Invocation()
    {
        var count = 0;
        using var debouncer = new Debouncer(TimeSpan.FromMilliseconds(100), () => Interlocked.Increment(ref count));

        for (var i = 0; i < 5; i++)
        {
            debouncer.Trigger();
            await Task.Delay(20, TestContext.Current.CancellationToken);
        }

        await Task.Delay(400, TestContext.Current.CancellationToken);

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task No_Trigger_Means_No_Invocation()
    {
        var count = 0;
        using var debouncer = new Debouncer(TimeSpan.FromMilliseconds(50), () => Interlocked.Increment(ref count));

        await Task.Delay(200, TestContext.Current.CancellationToken);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Disposed_Debouncer_Does_Not_Invoke()
    {
        var count = 0;
        var debouncer = new Debouncer(TimeSpan.FromMilliseconds(50), () => Interlocked.Increment(ref count));

        debouncer.Trigger();
        debouncer.Dispose();
        await Task.Delay(200, TestContext.Current.CancellationToken);

        Assert.Equal(0, count);
    }
}
