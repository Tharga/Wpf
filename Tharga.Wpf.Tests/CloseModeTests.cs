using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class CloseModeTests
{
    [Theory]
    [InlineData(CloseMode.Default, 0)]
    [InlineData(CloseMode.Soft, 1)]
    [InlineData(CloseMode.Force, 2)]
    public void CloseMode_Has_Expected_Values(CloseMode mode, int expected)
    {
        Assert.Equal(expected, (int)mode);
    }

    [Fact]
    public void CloseMode_Has_Three_Members()
    {
        Assert.Equal(3, Enum.GetValues<CloseMode>().Length);
    }
}
