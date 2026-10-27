using System.ComponentModel;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class AsyncCloseGateTests
{
    [Fact]
    public void Pending_Check_Cancels_Before_The_Handler_Returns()
    {
        var sut = new AsyncCloseGate();
        var e = new CancelEventArgs();
        var check = new TaskCompletionSource<bool>();

        _ = sut.RunAsync(e, () => check.Task, () => { }, () => { });

        Assert.True(e.Cancel);
    }

    [Fact]
    public async Task Pending_Check_That_Allows_Closing_Closes_Again_Once()
    {
        var sut = new AsyncCloseGate();
        var e = new CancelEventArgs();
        var check = new TaskCompletionSource<bool>();
        var proceedCount = 0;
        var closeAgainCount = 0;

        var task = sut.RunAsync(e, () => check.Task, () => proceedCount++, () => closeAgainCount++);
        Assert.Equal(0, closeAgainCount);

        check.SetResult(true);
        await task;

        Assert.Equal(1, closeAgainCount);
        Assert.Equal(0, proceedCount);
        Assert.True(sut.TryConsumeConfirmed());
        Assert.False(sut.TryConsumeConfirmed());
    }

    [Fact]
    public async Task Pending_Check_That_Refuses_Keeps_The_Window_Open()
    {
        var sut = new AsyncCloseGate();
        var e = new CancelEventArgs();
        var check = new TaskCompletionSource<bool>();
        var closeAgainCount = 0;

        var task = sut.RunAsync(e, () => check.Task, () => { }, () => closeAgainCount++);
        check.SetResult(false);
        await task;

        Assert.True(e.Cancel);
        Assert.Equal(0, closeAgainCount);
        Assert.False(sut.TryConsumeConfirmed());
    }

    [Fact]
    public async Task Synchronous_Check_That_Allows_Closing_Proceeds_In_The_Same_Pass()
    {
        var sut = new AsyncCloseGate();
        var e = new CancelEventArgs();
        var proceedCount = 0;
        var closeAgainCount = 0;

        await sut.RunAsync(e, () => Task.FromResult(true), () => proceedCount++, () => closeAgainCount++);

        Assert.False(e.Cancel);
        Assert.Equal(1, proceedCount);
        Assert.Equal(0, closeAgainCount);
        Assert.False(sut.TryConsumeConfirmed());
    }

    [Fact]
    public async Task Synchronous_Check_That_Refuses_Cancels()
    {
        var sut = new AsyncCloseGate();
        var e = new CancelEventArgs();
        var proceedCount = 0;

        await sut.RunAsync(e, () => Task.FromResult(false), () => proceedCount++, () => { });

        Assert.True(e.Cancel);
        Assert.Equal(0, proceedCount);
    }

    [Fact]
    public async Task Close_While_A_Check_Is_Pending_Is_Cancelled_Without_Checking_Again()
    {
        var sut = new AsyncCloseGate();
        var check = new TaskCompletionSource<bool>();
        var checkCount = 0;
        Func<Task<bool>> canCloseAsync = () =>
        {
            checkCount++;
            return check.Task;
        };

        var first = sut.RunAsync(new CancelEventArgs(), canCloseAsync, () => { }, () => { });
        var second = new CancelEventArgs();
        var secondTask = sut.RunAsync(second, canCloseAsync, () => { }, () => { });

        Assert.True(second.Cancel);
        Assert.Equal(1, checkCount);

        check.SetResult(true);
        await first;
        await secondTask;
    }

    [Fact]
    public async Task Failed_Check_Allows_A_New_Check()
    {
        var sut = new AsyncCloseGate();
        var check = new TaskCompletionSource<bool>();

        var first = sut.RunAsync(new CancelEventArgs(), () => check.Task, () => { }, () => { });
        check.SetException(new InvalidOperationException());
        await Assert.ThrowsAsync<InvalidOperationException>(() => first);

        var e = new CancelEventArgs();
        var proceedCount = 0;
        await sut.RunAsync(e, () => Task.FromResult(true), () => proceedCount++, () => { });

        Assert.False(e.Cancel);
        Assert.Equal(1, proceedCount);
    }
}
