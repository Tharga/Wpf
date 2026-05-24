using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class UpdateSystemTests
{
    [Theory]
    [InlineData(UpdateSystem.None, 0)]
    [InlineData(UpdateSystem.Velopack, 1)]
    public void UpdateSystem_Has_Expected_Values(UpdateSystem system, int expected)
    {
        Assert.Equal(expected, (int)system);
    }

    [Fact]
    public void UpdateSystem_Has_Two_Members()
    {
        Assert.Equal(2, Enum.GetValues<UpdateSystem>().Length);
    }
}
