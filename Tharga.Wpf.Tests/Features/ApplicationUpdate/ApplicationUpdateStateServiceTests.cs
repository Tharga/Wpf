using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.License;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.Tests.Features.ApplicationUpdate;

public class ApplicationUpdateStateServiceTests
{
    private const string DebuggerSkipReason = "Update checks are skipped while a debugger is attached.";
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task A_Check_Started_While_Another_Runs_Is_Skipped_And_Leaves_The_Running_Check_Alone()
    {
        Assert.SkipWhen(Debugger.IsAttached, DebuggerSkipReason);
        var download = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var sut = CreateService(update: () => download.Task);
        var splashCompleteCount = 0;
        sut.SplashCompleteEvent += (_, _) => Interlocked.Increment(ref splashCompleteCount);

        var first = sut.CheckForUpdateAsync("first");
        await sut.CheckForUpdateAsync("second").WaitAsync(Timeout, TestContext.Current.CancellationToken);
        var third = sut.CheckForUpdateAsync("third");

        Assert.Equal(1, sut.UpdateCount);
        Assert.Equal(0, splashCompleteCount);
        Assert.False(first.IsCompleted);

        download.SetResult();
        await first.WaitAsync(Timeout, TestContext.Current.CancellationToken);
        await third.WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(1, sut.UpdateCount);
        Assert.Equal(1, splashCompleteCount);
    }

    [Fact]
    public async Task A_Check_Runs_Again_Once_The_Previous_Check_Has_Finished()
    {
        Assert.SkipWhen(Debugger.IsAttached, DebuggerSkipReason);
        var sut = CreateService(update: () => Task.CompletedTask);

        await sut.CheckForUpdateAsync("first").WaitAsync(Timeout, TestContext.Current.CancellationToken);
        await sut.CheckForUpdateAsync("second").WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(2, sut.UpdateCount);
    }

    [Fact]
    public async Task A_Failed_Check_Releases_The_Lock_For_The_Next_Check()
    {
        Assert.SkipWhen(Debugger.IsAttached, DebuggerSkipReason);
        var fail = true;
        var sut = CreateService(update: () => fail ? Task.FromException(new InvalidOperationException("Update failed.")) : Task.CompletedTask);

        await sut.CheckForUpdateAsync("first").WaitAsync(Timeout, TestContext.Current.CancellationToken);
        fail = false;
        await sut.CheckForUpdateAsync("second").WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(2, sut.UpdateCount);
    }

    private static TestUpdateStateService CreateService(Func<Task>? update = null, ITabNavigationStateService? tabNavigation = null)
    {
        var downloadService = new Mock<IApplicationDownloadService>();
        downloadService.Setup(x => x.GetApplicationLocationAsync()).ReturnsAsync(("https://updates.example", "test"));

        return StaThread.Run(() => new TestUpdateStateService(
            downloadService.Object,
            tabNavigation ?? Mock.Of<ITabNavigationStateService>(),
            new Window(),
            update ?? (() => Task.CompletedTask)));
    }

    private sealed class TestUpdateStateService : ApplicationUpdateStateServiceBase
    {
        private readonly Func<Task> _update;
        private int _updateCount;

        public TestUpdateStateService(IApplicationDownloadService downloadService, ITabNavigationStateService tabNavigation, Window mainWindow, Func<Task> update)
            : base(new ConfigurationBuilder().Build(), NullLoggerFactory.Instance, Mock.Of<ILicenseClient>(), downloadService, tabNavigation, new ThargaWpfOptions { ApplicationShortName = "Test" }, mainWindow)
        {
            _update = update;
        }

        public int UpdateCount => Volatile.Read(ref _updateCount);

        protected override Task UpdateAsync(string clientLocation)
        {
            Interlocked.Increment(ref _updateCount);
            return _update();
        }
    }
}
