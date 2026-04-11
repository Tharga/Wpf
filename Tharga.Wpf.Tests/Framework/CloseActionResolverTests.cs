using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests.Framework;

public class CloseActionResolverTests
{
    [Fact]
    public void Default_WithHideOnClose_Returns_Hide()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Default, hideOnClose: true);
        Assert.Equal(CloseAction.Hide, result);
    }

    [Fact]
    public void Default_WithoutHideOnClose_Returns_CloseSoft()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Default, hideOnClose: false);
        Assert.Equal(CloseAction.CloseSoft, result);
    }

    [Fact]
    public void Soft_WithHideOnClose_Returns_CloseSoft()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Soft, hideOnClose: true);
        Assert.Equal(CloseAction.CloseSoft, result);
    }

    [Fact]
    public void Soft_WithoutHideOnClose_Returns_CloseSoft()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Soft, hideOnClose: false);
        Assert.Equal(CloseAction.CloseSoft, result);
    }

    [Fact]
    public void Force_WithHideOnClose_Returns_CloseForce()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Force, hideOnClose: true);
        Assert.Equal(CloseAction.CloseForce, result);
    }

    [Fact]
    public void Force_WithoutHideOnClose_Returns_CloseForce()
    {
        var result = CloseActionResolver.Resolve(CloseMode.Force, hideOnClose: false);
        Assert.Equal(CloseAction.CloseForce, result);
    }

    [Fact]
    public void Invalid_CloseMode_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CloseActionResolver.Resolve((CloseMode)99, hideOnClose: false));
    }
}
