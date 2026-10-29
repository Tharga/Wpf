using Tharga.Wpf.Framework;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Framework;

public class ScreenScalingTests
{
    private const double Dpi100Percent = 96;
    private const double Dpi125Percent = 120;
    private const double Dpi150Percent = 144;

    [Theory]
    [InlineData(0, 0, 1920, 1040)]
    [InlineData(-1920, 0, 1920, 1040)]
    [InlineData(1920, -200, 2560, 1400)]
    [InlineData(1366, 0, 1366, 728)]
    public void At_100_Percent_The_Work_Area_Is_Unchanged(int left, int top, int width, int height)
    {
        var bounds = new ScreenBounds(left, top, width, height, true);

        var result = ScreenScaling.ToDeviceIndependent(bounds, Dpi100Percent);

        Assert.Equal(bounds, result);
    }

    [Fact]
    public void At_150_Percent_The_Work_Area_Is_Converted_To_Device_Independent_Units()
    {
        var result = ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1080, true), Dpi150Percent);

        Assert.Equal(new ScreenBounds(0, 0, 1280, 720, true), result);
    }

    [Fact]
    public void At_125_Percent_The_Work_Area_Is_Converted_To_Device_Independent_Units()
    {
        var result = ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1080), Dpi125Percent);

        Assert.Equal(new ScreenBounds(0, 0, 1536, 864), result);
    }

    [Fact]
    public void The_Origin_Of_A_Secondary_Monitor_Is_Scaled_By_That_Monitors_Dpi()
    {
        var result = ScreenScaling.ToDeviceIndependent(new ScreenBounds(-2880, -300, 2880, 1620), Dpi150Percent);

        Assert.Equal(new ScreenBounds(-1920, -200, 1920, 1080), result);
    }

    [Fact]
    public void A_Converted_Work_Area_Never_Extends_Beyond_The_Monitor()
    {
        var result = ScreenScaling.ToDeviceIndependent(new ScreenBounds(1, 1, 1366, 767), Dpi125Percent);

        Assert.Equal(new ScreenBounds(1, 1, 1092, 613), result);
        Assert.True(result.Left * 1.25 >= 1);
        Assert.True((result.Left + result.Width) * 1.25 <= 1 + 1366);
        Assert.True((result.Top + result.Height) * 1.25 <= 1 + 767);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    public void An_Unknown_Dpi_Is_Treated_As_100_Percent(double dpi)
    {
        var bounds = new ScreenBounds(0, 0, 1920, 1080);

        var result = ScreenScaling.ToDeviceIndependent(bounds, dpi);

        Assert.Equal(bounds, result);
    }

    [Fact]
    public void GetDpi_Reads_The_Dpi_Of_The_Monitor_At_A_Point()
    {
        var dpi = ScreenScaling.GetDpi(new System.Drawing.Rectangle(0, 0, 1, 1));

        Assert.True(dpi >= Dpi100Percent, $"DPI was {dpi}.");
    }

    [Fact]
    public void A_Location_Left_On_A_Disconnected_Monitor_Is_Rescued_On_A_Scaled_Primary_Screen()
    {
        var screens = new[] { ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1080, true), Dpi150Percent) };
        var location = new Location { Left = 1400, Top = 100, Width = 800, Height = 450 };

        var result = LocationValidator.Validate(location, screens);

        Assert.Equal(240, result.Left);
        Assert.Equal(135, result.Top);
        Assert.Equal(800, result.Width);
        Assert.Equal(450, result.Height);
    }

    [Fact]
    public void A_Rescued_Location_Is_Clamped_And_Centred_In_Device_Independent_Units()
    {
        var screens = new[] { ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1080, true), Dpi150Percent) };
        var location = new Location { Left = 5000, Top = 100, Width = 1500, Height = 900 };

        var result = LocationValidator.Validate(location, screens);

        Assert.Equal(0, result.Left);
        Assert.Equal(0, result.Top);
        Assert.Equal(1280, result.Width);
        Assert.Equal(720, result.Height);
    }

    [Fact]
    public void A_Location_On_A_Scaled_Screen_Is_Kept()
    {
        var screens = new[] { ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1080, true), Dpi150Percent) };
        var location = new Location { Left = 100, Top = 100, Width = 800, Height = 450 };

        var result = LocationValidator.Validate(location, screens);

        Assert.Same(location, result);
    }

    [Fact]
    public void At_100_Percent_A_Location_That_Is_Valid_Today_Restores_To_The_Same_Place()
    {
        var screens = new[]
        {
            ScreenScaling.ToDeviceIndependent(new ScreenBounds(0, 0, 1920, 1040, true), Dpi100Percent),
            ScreenScaling.ToDeviceIndependent(new ScreenBounds(1920, 0, 1920, 1040), Dpi100Percent)
        };
        var location = new Location { Left = 1800, Top = 50, Width = 900, Height = 600 };

        var result = LocationValidator.Validate(location, screens);

        Assert.Same(location, result);
    }
}
