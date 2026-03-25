using Tharga.Wpf.Dialog;

namespace Tharga.Wpf.Tests.Features.Dialog;

public class RequestCloseEventTests
{
    [Fact]
    public void Constructor_Sets_DialogResult_True()
    {
        var evt = new RequestCloseEvent(true);

        Assert.True(evt.DialogResult);
    }

    [Fact]
    public void Constructor_Sets_DialogResult_False()
    {
        var evt = new RequestCloseEvent(false);

        Assert.False(evt.DialogResult);
    }
}
