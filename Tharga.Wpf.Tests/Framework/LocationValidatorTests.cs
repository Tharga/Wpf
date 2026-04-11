using Tharga.Wpf.Framework;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Tests.Framework;

public class LocationValidatorTests
{
    private static readonly IReadOnlyList<ScreenBounds> SingleScreen =
        [new ScreenBounds(0, 0, 1920, 1080)];

    private static readonly IReadOnlyList<ScreenBounds> DualScreen =
        [new ScreenBounds(0, 0, 1920, 1080), new ScreenBounds(1920, 0, 1920, 1080)];

    [Fact]
    public void Valid_Location_Returns_Unchanged()
    {
        var location = new Location { Left = 100, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(100, result.Left);
        Assert.Equal(100, result.Top);
        Assert.Equal(800, result.Width);
        Assert.Equal(450, result.Height);
    }

    [Fact]
    public void Zero_Width_Corrected_To_Default()
    {
        var location = new Location { Left = 100, Top = 100, Width = 0, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(800, result.Width);
    }

    [Fact]
    public void Zero_Height_Corrected_To_Default()
    {
        var location = new Location { Left = 100, Top = 100, Width = 800, Height = 0 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(450, result.Height);
    }

    [Fact]
    public void Negative_Size_Corrected_To_Default()
    {
        var location = new Location { Left = 100, Top = 100, Width = -1, Height = -1 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(800, result.Width);
        Assert.Equal(450, result.Height);
    }

    [Fact]
    public void Off_Screen_Right_Repositioned_To_Center()
    {
        var location = new Location { Left = 5000, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(560, result.Left);
        Assert.Equal(315, result.Top);
    }

    [Fact]
    public void Off_Screen_Above_Repositioned_To_Center()
    {
        var location = new Location { Left = 100, Top = -2000, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(560, result.Left);
        Assert.Equal(315, result.Top);
    }

    [Fact]
    public void On_Second_Monitor_Stays_There()
    {
        var location = new Location { Left = 2000, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, DualScreen);

        Assert.Equal(2000, result.Left);
        Assert.Equal(100, result.Top);
    }

    [Fact]
    public void On_Disconnected_Second_Monitor_Repositioned_To_Primary()
    {
        var location = new Location { Left = 2000, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(560, result.Left);
        Assert.Equal(315, result.Top);
    }

    [Fact]
    public void Null_Location_Returns_Null()
    {
        var result = LocationValidator.Validate(null, SingleScreen);
        Assert.Null(result);
    }

    [Fact]
    public void Empty_Screens_Returns_Unchanged()
    {
        var location = new Location { Left = 100, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, []);

        Assert.Equal(100, result.Left);
    }

    [Fact]
    public void Custom_Defaults_Used_For_Invalid_Size()
    {
        var location = new Location { Left = 100, Top = 100, Width = 0, Height = 0 };
        var result = LocationValidator.Validate(location, SingleScreen, defaultWidth: 1024, defaultHeight: 768);

        Assert.Equal(1024, result.Width);
        Assert.Equal(768, result.Height);
    }

    [Fact]
    public void Partially_Visible_Window_Stays()
    {
        // Window at edge but at least 50px visible
        var location = new Location { Left = 1860, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(1860, result.Left);
    }

    [Fact]
    public void Barely_Off_Screen_Repositioned()
    {
        // Window only 10px visible — not enough
        var location = new Location { Left = 1910, Top = 100, Width = 800, Height = 450 };
        var result = LocationValidator.Validate(location, SingleScreen);

        Assert.Equal(560, result.Left);
    }
}
