using System.ComponentModel;
using System.Windows;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class WindowCloserTests
{
    [Fact]
    public void Close_Returns_True_When_The_Window_Closes()
    {
        var result = StaThread.Run(() => WindowCloser.Close(new Window()));

        Assert.True(result);
    }

    [Fact]
    public void Close_Returns_False_When_A_Closing_Handler_Cancels()
    {
        var result = StaThread.Run(() => WindowCloser.Close(new CancellingWindow()));

        Assert.False(result);
    }

    [Fact]
    public void Close_Returns_False_When_The_Close_Is_Deferred()
    {
        var result = StaThread.Run(() =>
        {
            var window = new Window();
            window.Closing += (_, e) => e.Cancel = true;
            return WindowCloser.Close(window);
        });

        Assert.False(result);
    }

    private sealed class CancellingWindow : Window
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
