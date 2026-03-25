using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class UpdateSystemTests
{
    [Theory]
    [InlineData(UpdateSystem.None, 0)]
    [InlineData(UpdateSystem.Squirrel, 1)]
    [InlineData(UpdateSystem.Velopack, 2)]
    public void UpdateSystem_Has_Expected_Values(UpdateSystem system, int expected)
    {
        Assert.Equal(expected, (int)system);
    }

    [Fact]
    public void UpdateSystem_Has_Three_Members()
    {
        Assert.Equal(3, Enum.GetValues<UpdateSystem>().Length);
    }
}
