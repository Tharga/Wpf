using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Tests.Features.ApplicationUpdate;

public class VelopackUpdateMessageTests
{
    [Fact]
    public void Single_Delta()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 1, deltaSize: 100, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (delta)", result);
    }

    [Fact]
    public void Multiple_Deltas()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 3, deltaSize: 300, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (3 deltas)", result);
    }

    [Fact]
    public void No_Deltas_Is_Full()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 0, deltaSize: 0, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (full)", result);
    }

    [Fact]
    public void More_Deltas_Than_Fallback_Limit_Is_Full()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 11, deltaSize: 300, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (full)", result);
    }

    [Fact]
    public void Delta_Count_At_Fallback_Limit_Is_Still_Delta()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 10, deltaSize: 300, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (10 deltas)", result);
    }

    [Fact]
    public void Deltas_Larger_Than_Full_Package_Is_Full()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 3, deltaSize: 1500, fullSize: 1000, maximumDeltasBeforeFallback: 10);

        Assert.Equal("version 2.4.1 (full)", result);
    }

    [Fact]
    public void Deltas_Disabled_By_Negative_Limit_Is_Full()
    {
        var result = VelopackUpdateMessage.Build("2.4.1", deltaCount: 1, deltaSize: 100, fullSize: 1000, maximumDeltasBeforeFallback: -1);

        Assert.Equal("version 2.4.1 (full)", result);
    }
}
