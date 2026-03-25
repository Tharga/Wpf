using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class BeforeCloseEventArgsTests
{
    [Fact]
    public void Cancel_Defaults_To_False()
    {
        var args = new BeforeCloseEventArgs();

        Assert.False(args.Cancel);
    }

    [Fact]
    public void Cancel_Can_Be_Set_To_True()
    {
        var args = new BeforeCloseEventArgs { Cancel = true };

        Assert.True(args.Cancel);
    }
}
